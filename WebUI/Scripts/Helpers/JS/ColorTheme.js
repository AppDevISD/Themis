const setStoredColorThemeCookie = theme => document.cookie = `colorTheme=${theme}; expires=${cookieExpiration}`
$(document).ready(function () {
	var colorTheme = document.documentElement.getAttribute('data-color-theme');
	setStoredColorThemeCookie(document.documentElement.getAttribute('data-color-theme'));
	document.getElementById(`${colorTheme}Switch`).checked = true;
});
function ChangeColor(color) {
	document.documentElement.setAttribute('data-color-theme', color);
	setStoredColorThemeCookie(document.documentElement.getAttribute('data-color-theme'));
}