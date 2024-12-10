function pageLoad(sender, args) {
	var requiredFields = document.querySelectorAll('[required="true"]');
	if (requiredFields.length > 0) {
		for (const field of requiredFields) {
			const id = field.id;
			const label = $(`[for="${id}"]`);
			$(label).append('<span class="required-field"> *</span>');
		}
	}

	var activePage = document.getElementsByClassName('activePage');
	for (const element of activePage) {
		element.parentElement.classList.add('show');
	}
}