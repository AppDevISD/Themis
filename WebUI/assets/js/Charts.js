const lineData = {
	labels: Utils.months({ count: 12 }),
	datasets: [
		{
			label: 'Tension',
			fill: false,
			tension: 5,
			lineTension: 0.5,
			pointRadius: 4,
			pointBorderColor: 'white',
			data: Utils.numbers({ count: 12, min: 0, max: 100 })
		},
		{
			label: 'No Tension',
			fill: false,
			pointRadius: 4,
			pointBorderColor: 'white',
			data: Utils.numbers({ count: 12, min: 0, max: 100 })
		}
	]
}
const barData = {
	labels: Utils.months({ count: 12 }),
	datasets: [
		{
			label: 'Squared',
			borderRadius: 0,
			data: Utils.numbers({ count: 12, min: 0, max: 100 })
		},
		{
			label: 'Slightly Rounded',
			borderRadius: 5,
			data: Utils.numbers({ count: 12, min: 0, max: 100 })
		},
		{
			label: 'More Rounded',
			borderRadius: 7,
			data: Utils.numbers({ count: 12, min: 0, max: 100 })
		},
		{
			label: 'Rounded',
			borderRadius: Number.MAX_VALUE,
			data: Utils.numbers({ count: 12, min: 0, max: 100 })
		}
	]
}
const baseOptions = (axis) => {
	return {
		indexAxis: axis,
		legend: {
			labels: {
				display: true,
				color: function () { return textColor() }
			}
		},
		scales: {
			x: {
				title: {
					display: true,
					text: 'Months'
				},
				grid: {
					color: function () { return borderColor() }
				},
				ticks: {
					min: 0,
					max: 100,
					color: function () { return textColor() }
				}
			},
			y: {
				title: {
					display: true,
					text: 'Numbers'
				},
				grid: {
					color: function () { return borderColor() }
				},
				ticks: {
					min: 0,
					max: 100,
					color: function () { return textColor() }
				}
			}
		},
		plugins: {
			title: {
				display: true,
				text: 'Graph Title',
				color: function () { return textColor() }
			},
			autocolors: {
				mode: 'label',
				offset: 0,
				customize: function (context) {
					const colors = context.colors;
					return {
						background: opacity(colors.background, 1.0),
						border: opacity(colors.background, 0.65),
					}
				}
			}
		}
}
}

const getRandomCoordsCSharp = (count, min, max) => {
	var dataString = JSON.stringify({ count: count, min: min, max: max });
	var valArray = [];
	$.ajax({
		type: "POST",
		async: false,
		url: "./Scripts/Helpers/CSharp/ChartHandler.asmx/SaveFileOnPostback",
		data: dataString,
		contentType: "application/json",
		dataType: "json",
		success: function (response) {
			valArray = JSON.parse(response.d);
		}
	});
	return valArray;
};



// CHARTS //
const charts = [
	new Chart('lineChart', {
		type: "line",
		data: lineData,
		options: baseOptions("x")
	}),
	new Chart('barChart', {
		type: "bar",
		data: barData,
		options: baseOptions("x")
	}),
];