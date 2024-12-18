// PLUGINS //
const Utils = ChartUtils.init();
const autocolors = window['chartjs-plugin-autocolors'];
Chart.register(autocolors);

// COLOR FUNCTIONS //
const opacity = (color, value) => Chart.helpers.color(color).alpha(value).rgbString();
const borderColor = () => getComputedStyle(document.body).getPropertyValue('--chart-border-color');
const textColor = () => getComputedStyle(document.body).getPropertyValue('--bs-body-color');
const lineColor = () => getComputedStyle(document.body).getPropertyValue('--chart-line-color');
Chart.defaults.color = textColor();



// DEV OPTIONS VARIABLES (ONLY USED FOR SHOWING AVAILABLE OPTIONS IN DEVELOPMENT) //
var colorLinesBool = true;
var pointBorderBool = true;
var fillBool = false;
var stackedBool = false;
var stackedGroupBool = false;
var dataSource = "js";
const monthsLabel = Utils.months({ count: 12 });



// CODE THEME FUNCTIONS (ONLY USED FOR DEVELOPMENT) //
$(document).ready(function () {
	switch (document.documentElement.getAttribute('data-bs-theme')) {
		case 'light':
			$("[data-code-theme]").attr("href", "https://cdn.jsdelivr.net/npm/prismjs@1.29.0/themes/prism.min.css");
			$("[data-code-theme]").attr("data-code-theme", "light");
			break;
		case 'dark':
			$("[data-code-theme]").attr("href", "https://cdn.jsdelivr.net/npm/prismjs@1.29.0/themes/prism-tomorrow.min.css");
			$("[data-code-theme]").attr("data-code-theme", "dark");
			break;
	}
})

$("[data-bs-theme]").change(function () {
	switch (document.documentElement.getAttribute('data-bs-theme')) {
		case 'light':
			$("[data-code-theme]").attr("href", "https://cdn.jsdelivr.net/npm/prismjs@1.29.0/themes/prism.min.css");
			$("[data-code-theme]").attr("data-code-theme", "light");
			break;
		case 'dark':
			$("[data-code-theme]").attr("href", "https://cdn.jsdelivr.net/npm/prismjs@1.29.0/themes/prism-tomorrow.min.css");
			$("[data-code-theme]").attr("data-code-theme", "dark");
			break;
	}
});