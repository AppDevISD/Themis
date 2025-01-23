function pageLoad(sender, args) {
	var requiredFields = document.querySelectorAll('[required="true"]');
	var readonly;
	try {
		var readonlyDiv = document.getElementById('ordView');
		if (readonlyDiv.getAttribute("readonly") == "true") {
			console.log(readonlyDiv.getAttribute("readonly"));
			readonly = true;
		}
		else {
			readonly = false;
		}
	} catch (e) {
		readonly = false;
	}
	if (requiredFields.length > 0 && !readonly) {
		var alreadyLabeled = false;
		for (const field of requiredFields) {
			const parent = field.parentElement;
			const id = parent.id;
			if (parent.tagName == 'TD') {
				var column = $(parent).closest('table').find('th').eq($(parent).index());
				if ($(parent).parent().index() < 1) {
					$(column).append('<span class="required-field"> *</span>');
				}
			}
			else if (parent.classList.contains('input-group') && id != '' && !alreadyLabeled) {
				const label = $(`[for="${id}"]`);
				$(label).append('<span class="required-field"> *</span>');
				alreadyLabeled = true;
			}
			else if (alreadyLabeled) {
				alreadyLabeled = false;
			}
			else {
				const id = field.id;
				const label = $(`[for="${id}"]`);
				$(label).append('<span class="required-field"> *</span>');
			}
		}
	}

	var activePage = document.getElementsByClassName('activePage');
	for (const element of activePage) {
		element.parentElement.classList.add('show');
	}
}