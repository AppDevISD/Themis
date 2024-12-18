const cookieExpiration = new Date(new Date().setFullYear(new Date().getFullYear() + 1));
const setStoredThemeCookie = theme => document.cookie = `userTheme=${theme}; expires=${cookieExpiration}`;
function setChartColors() {
	Chart.defaults.color = textColor();
	for (var v in charts) {
		charts[v].update();
	}
}

$(document).ready(function () {
	switch (document.documentElement.getAttribute('data-bs-theme')) {
		case 'light':
			document.getElementById('themeSwitch').checked = false;
			break;
		case 'dark':
			document.getElementById('themeSwitch').checked = true;
			break;
	}

	try { setChartColors(); } catch (error) { }
})

$("#themeSwitch").change(function () {
	switch (this.checked) {
		case true:
			document.documentElement.setAttribute('data-bs-theme', 'dark');
			break;
		case false:
			document.documentElement.setAttribute('data-bs-theme', 'light');

			break;
	}

	try { setChartColors(); } catch (error) {  }

	setStoredThemeCookie(document.documentElement.getAttribute('data-bs-theme'));
})