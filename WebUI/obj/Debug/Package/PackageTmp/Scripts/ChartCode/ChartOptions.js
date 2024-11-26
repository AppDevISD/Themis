import dataOptions from './DataOptions.json' with { type: 'json' };

$('.chart-control').on('click', function () {
		let chartID = $(this).attr("data-chart");
		let chart = Chart.getChart(chartID);
		var control = $(this).attr("data-control");
		var val = (control == "randomize") ? "randomize" : this.checked;
		var chartOptions = chart.options;
		switch (val) {
			case true:
				switch (control) {
					// ALL CHARTS //
					case "title":
						chart.options.plugins.title.display = true;
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: true });
						break;
					case "xTitle":
						chart.options.scales.x.title.display = true;
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: true });
						break;
					case "yTitle":
						chart.options.scales.y.title.display = true;
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: true });
						break;
					case "orientation":
						var chartData = chart.data;
						var chartType = chart.config.type;
						setSwitches(chartID);
						chart.destroy();
						new Chart(chartID, {
							type: chartType,
							data: chartData,
							options: baseOptions("y")
						});
						$(this).siblings().text("Vertical");
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: "'y'" });
						break;
					case "dataSource":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							chart.data.datasets[i].data = getRandomCoordsCSharp(12, 0, 100);
						}
						dataSource = "c#";
						var functionVal = "getRandomCoordsCSharp(12, 0, 100)";
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: functionVal });
						$(`[data-source-hide=${chartID}]`).show();
						$(this).siblings().text("C#");
						break;

					// LINES //
					case "colorLines":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							const dataset = chart.data.datasets[i];
							chart.data.datasets[i].borderColor = opacity(dataset.backgroundColor, 0.65);
							if (!pointBorderBool) {
								chart.data.datasets[i].pointBorderColor = opacity(dataset.backgroundColor, 1.0);
							}
						}
						var functionVal
						if (fillBool) {
							functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 0.75),\n\t\t\t\t\tborder: opacity(colors.background, 1.0)\n\t\t\t\t}\n\t\t\t}";
						}
						else {
							functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 1.0),\n\t\t\t\t\tborder: opacity(colors.background, 0.65)\n\t\t\t\t}\n\t\t\t}";
						}
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: functionVal });
						colorLinesBool = true;
						break;
					case "pointBorder":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							chart.data.datasets[i].pointBorderColor = "white";
						}
						pointBorderBool = true;
						var functionVal = "set";
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: functionVal, val2: "'white'" });
						break;
					case "fill":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							const dataset = chart.data.datasets[i];
							chart.data.datasets[i].fill = true;
							chart.data.datasets[i].backgroundColor = opacity(dataset.backgroundColor, 0.75);
							var functionVal;
							if (colorLinesBool) {
								chart.data.datasets[i].borderColor = opacity(dataset.backgroundColor, 1.0);
								functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 0.75),\n\t\t\t\t\tborder: opacity(colors.background, 1.0)\n\t\t\t\t}\n\t\t\t}";
							}
							else {
								functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 0.75)\n\t\t\t\t}\n\t\t\t}";
							}
							setCodeBlocks({ chartID: chartID, update: true, control: control, val1: true, val2: functionVal });
							fillBool = true;
						}
						break;

					// BARS //
					case "stacked":
						chart.options.scales.x.stacked = true;
						chart.options.scales.y.stacked = true;
						stackedBool = true;
						break;
					case "stackedGroup":
						const stackedControl = $(`[data-chart=${chartID}][data-control="stacked"]`);
						$(stackedControl).prop('checked', true);
						$(stackedControl).prop('disabled', true);
						chart.options.scales.x.stacked = true;
						chart.options.scales.y.stacked = true;
						chart.data.datasets[0].stack = "Group 1";
						chart.data.datasets[1].stack = "Group 1";
						chart.data.datasets[2].stack = "Group 2";
						chart.data.datasets[3].stack = "Group 2";
						stackedGroupBool = true;
						break;
				}
				break;
			case false:
				switch (control) {
					// ALL CHARTS //
					case "title":
						chart.options.plugins.title.display = false;
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: false });
						break;
					case "xTitle":
						chart.options.scales.x.title.display = false;
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: false });
						break;
					case "yTitle":
						chart.options.scales.y.title.display = false;
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: false });
						break;
					case "orientation":
						var chartData = chart.data;
						var chartType = chart.config.type;
						setSwitches(chartID);
						chart.destroy();
						new Chart(chartID, {
							type: chartType,
							data: chartData,
							options: baseOptions("x")
						});
						$(this).siblings().text("Horizontal");
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: "'x'" });
						/*					firstSwitch = true;*/
						break;
					case "dataSource":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							chart.data.datasets[i].data = Utils.numbers({ count: 12, min: 0, max: 100 });
						}
						dataSource = "js";
						var functionVal = "Utils.numbers({ count: 12, min: 0, max: 100 })";
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: functionVal });
						$(`[data-source-hide=${chartID}]`).hide();
						if ($(`[data-source-hide=${chartID}]`).hasClass('active')) {
							$(`[data-source-active=${chartID}]`).click();
						}
						$(this).siblings().text("JS");
						break;

					// LINES //
					case "colorLines":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							chart.data.datasets[i].borderColor = lineColor();
							if (!pointBorderBool) {
								chart.data.datasets[i].pointBorderColor = lineColor();
							}
						}
						var functionVal
						if (fillBool) {
							functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 0.75)\n\t\t\t\t}\n\t\t\t}";
						}
						else {
							functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 1.0)\n\t\t\t\t}\n\t\t\t}";
						}
						setCodeBlocks({ chartID: chartID, update: true, control: control, val1: functionVal });
						colorLinesBool = false;
						break;
					case "pointBorder":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							const dataset = chart.data.datasets[i];
							if (colorLinesBool) {
								chart.data.datasets[i].pointBorderColor = opacity(dataset.backgroundColor, 1.0);
							}
							else {
								chart.data.datasets[i].pointBorderColor = lineColor();
							}
							var functionVal = "delete";
							setCodeBlocks({ chartID: chartID, update: true, control: control, val1: functionVal });
						}

						pointBorderBool = false;
						break;
					case "fill":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							const dataset = chart.data.datasets[i];
							chart.data.datasets[i].fill = false;
							chart.data.datasets[i].backgroundColor = opacity(dataset.backgroundColor, 1.0);
							var functionVal;
							if (colorLinesBool) {
								chart.data.datasets[i].borderColor = opacity(dataset.backgroundColor, 0.65);
								functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 1.0),\n\t\t\t\t\tborder: opacity(colors.background, 0.65)\n\t\t\t\t}\n\t\t\t}";
							}
							else {
								functionVal = "function(context) {\n\t\t\t\tconst colors = context.colors;\n\t\t\t\treturn {\n\t\t\t\t\tbackground: opacity(colors.background, 1.0)\n\t\t\t\t}\n\t\t\t}";
							}
							setCodeBlocks({ chartID: chartID, update: true, control: control, val1: false, val2: functionVal });
							fillBool = false;
						}
						break;

					// BARS //
					case "stacked":
						if (!stackedGroupBool) {
							chart.options.scales.x.stacked = false;
							chart.options.scales.y.stacked = false;
						}
						stackedBool = false;
						break;
					case "stackedGroup":
						const stackedControl = $(`[data-chart=${chartID}][data-control="stacked"]`);
						$(stackedControl).prop('disabled', false);
						if (!stackedBool) {
							$(stackedControl).prop('checked', false);
							chart.options.scales.x.stacked = false;
							chart.options.scales.y.stacked = false;
						}
						for (var i = 0; i < chart.data.datasets.length; i++) {
							delete chart.data.datasets[i].stack;
						}
						stackedGroupBool = false;
						break;
				}
				break;
			case "randomize":
				switch (dataSource) {
					case "c#":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							const dataset = chart.data.datasets[i];
							chart.data.datasets[i].data = getRandomCoordsCSharp(12, 0, 100);
						}
						break;
					case "js":
						for (var i = 0; i < chart.data.datasets.length; i++) {
							chart.data.datasets[i].data = Utils.numbers({ count: 12, min: 0, max: 100 });
						}
						break;
				}
				break;
		}

		try { chart.update(); } catch (error) { }
});
function setSwitches(chart) {
	const chartControls = $(`.chart-control [data-chart=${chart}]`);
	for (var i = 0; i < chartControls.length; i++) {
		control = chartControls[i];
		switch ($(control).attr("data-control")) {
			default:
				var getChecked = control.checked;
				control.checked = !getChecked;
				control.checked = getChecked;
				break;
			case "randomize":
				break;
		}
	}
}

function valueOrDefault(value, defaultValue) {
	return typeof value === 'undefined' ? defaultValue : value
}
var dataOptionsJSON = dataOptions;
function setCodeBlocks(config) {
	const cfg = config || {};
	const chartID = cfg.chartID;
	const update = valueOrDefault(cfg.update, false);

	let chart = Chart.getChart(chartID);
	let chartJSON = dataOptionsJSON[chartID];
	let codeBlocks = $(`[data-chart-code=${chartID}]`);

	switch (update) {

		case true:
			const control = cfg.control;
			const val1 = cfg.val1;
			const val2 = valueOrDefault(cfg.val2, "");
			switch (control) {
				case "title":
					dataOptionsJSON[chartID].options.plugins.title.display = val1;
					break;
				case "xTitle":
					dataOptionsJSON[chartID].options.scales.x.title.display = val1;
					break;
				case "yTitle":
					dataOptionsJSON[chartID].options.scales.y.title.display = val1;
					break;
				case "colorLines":
					dataOptionsJSON[chartID].options.plugins.autocolors.customize = val1;
					break;
				case "pointBorder":
					switch (val1) {
						case "set":
							for (var i = 0; i < dataOptionsJSON[chartID].data.datasets.length; i++) {
								dataOptionsJSON[chartID].data.datasets[i].pointBorderColor = val2;
							}
							break;
						case "delete":
							for (var i = 0; i < dataOptionsJSON[chartID].data.datasets.length; i++) {
								delete dataOptionsJSON[chartID].data.datasets[i].pointBorderColor;
							}
							break;
					}
					break;
				case "fill":
					for (var i = 0; i < dataOptionsJSON[chartID].data.datasets.length; i++) {
						const dataset = dataOptionsJSON[chartID].data.datasets[i];
						dataOptionsJSON[chartID].data.datasets[i].fill = val1;
						dataOptionsJSON[chartID].options.plugins.autocolors.customize = val2;
					}
					break;
				case "orientation":
					dataOptionsJSON[chartID].options.indexAxis = val1
					var xTitle = dataOptionsJSON[chartID].options.scales.x.title.text;
					var yTitle = dataOptionsJSON[chartID].options.scales.y.title.text;
					dataOptionsJSON[chartID].options.scales.x.title.text = yTitle;
					dataOptionsJSON[chartID].options.scales.y.title.text = xTitle;
					break;
				case "dataSource":
					for (var i = 0; i < dataOptionsJSON[chartID].data.datasets.length; i++) {
						dataOptionsJSON[chartID].data.datasets[i].data = val1;
					}
					break;
			}
			chartJSON = dataOptionsJSON[chartID];

			for (var i = 0; i < codeBlocks.length; i++) {
				const dataType = $(codeBlocks[i]).attr('data-code-type');
				var variableName;
				var dictionary;
				var newCodeString;
				switch (dataType) {
					default:
						variableName = `${chartID}${dataType.replace(/\w\S*/g, text => `${text.charAt(0).toUpperCase()}${text.substring(1).toLowerCase()}`)}`;
						dictionary = makeDictionary(chartJSON[dataType]);
						newCodeString = `const ${variableName} = ${dictionary}`;
						break;
					case "dataSource":
						var dataSource = $(codeBlocks[i]).attr('data-source-type')
						dictionary = chartJSON[dataType];
						newCodeString = makeDictionary(dictionary[dataSource]);
						break;
				}
				$(codeBlocks[i]).html(newCodeString);
			}
			break;

		case false:
			$('[data-source-hide]').hide();
			for (var i = 0; i < codeBlocks.length; i++) {
				const dataType = $(codeBlocks[i]).attr('data-code-type');
				var variableName;
				var dictionary;
				var newCodeString;
				switch (dataType) {
					default:
						variableName = `${chartID}${dataType.replace(/\w\S*/g, text => `${text.charAt(0).toUpperCase()}${text.substring(1).toLowerCase()}`)}`;
						dictionary = makeDictionary(chartJSON[dataType]);
						newCodeString = `const ${variableName} = ${dictionary}`;
						break;
					case "dataSource":
						var dataSource = $(codeBlocks[i]).attr('data-source-type')
						dictionary = chartJSON[dataType];
						newCodeString = makeDictionary(dictionary[dataSource]);
						break;
				}
				$(codeBlocks[i]).html(newCodeString);
			}
			break;
	}
	Prism.highlightAll();
}
const jsonStringify = (json) => JSON.stringify(json, null, '\t');
const makeDictionary = (string) => jsonStringify(string).replace(/"[^"]*"/g, (match) => match.replace(/"/g, "")).replace(/\\n/g, "\n").replace(/\\t/g, "\t").replace(/\\/g, '"');

for (var i = 0; i < charts.length; i++) {
	const chartID = charts[i].canvas.id;
	setCodeBlocks({ chartID: chartID });
}
