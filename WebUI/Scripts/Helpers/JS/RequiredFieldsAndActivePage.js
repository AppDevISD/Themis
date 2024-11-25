$(document).ready(function () {
	var requiredFields = document.querySelectorAll('[required="true"]');
	if (requiredFields.length > 0) {
		for (const field of requiredFields) {
			field.previousElementSibling.insertAdjacentHTML('beforeend', '<span class="required-field"> *</span>');
		}
	}

	var activePage = document.getElementsByClassName('activePage');
	for (const element of activePage) {
		element.parentElement.classList.add('show');
	}
});