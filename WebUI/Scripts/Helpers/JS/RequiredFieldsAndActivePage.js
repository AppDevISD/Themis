function pageLoad(sender, args) {
	var requiredFields = document.querySelectorAll('[required="true"]');
	var readonly;
	try {
		var readonlyDiv = document.getElementById('ordView');
		if (readonlyDiv.getAttribute("readonly") == "true") {
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

	var loadBtns = document.querySelectorAll('[data-load-btn="true"]');
	if (loadBtns.length > 0) {
		var alreadyAdded = false;
		for (const btn of loadBtns) {
			const parent = btn.parentElement;
			const id = parent.id;
			if (alreadyAdded) {
				alreadyAdded = false;
			}
			else {
				const id = btn.id
				const text = btn.getAttribute('data-load-text');
				const icon = btn.getAttribute('data-load-icon');
				const cssClass = btn.getAttribute('class');
				const style = btn.getAttribute('style');
				var preventType = function () {
					if (btn.getAttribute('data-prevent-type') == "readonly") {
						return "readonly='readonly'";
					}
					if (btn.getAttribute('data-prevent-type') == "disabled") {
						return "disabled='disabled'";
					}
				};
				$(parent).append(`<button id="${id}Loading" class="${cssClass}" style="${style}" ${preventType()} hidden><span class="fa-fade"><span class="fas ${icon} fa-bounce"></span>&nbsp;${text}...</span></button>`);
			}
		}
	}

	var activePage = document.getElementsByClassName('activePage');
	for (const element of activePage) {
		element.parentElement.classList.add('show');
	}
}