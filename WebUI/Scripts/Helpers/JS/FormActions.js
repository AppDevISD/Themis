function DisableDDInitialOption(ddIDDict) {
	$(ddIDDict).each(function () {
		const cfg = this || {};
		const element = document.getElementById(cfg.id);
		if (element != null) {
			if (element.options[0].selected) {
				element.style.color = `rgb(from var(--bs-body-color) r g b / ${cfg.opacity}%)`;
			}
			else {
				element.style.color = "unset";
			}
			element.options[0].disabled = true;
		}
	});
}

function FilterFirstItem() {
	$('[data-filter="true"]').each(function () {
		if ($(this).prop('selectedIndex') === 0 && !$(this).prop('disabled')) {
			$(this).css('color', 'rgb(from var(--bs-body-color) r g b / 75%)');
		}
		else if ($(this).prop('selectedIndex') === 0 && $(this).prop('disabled')) {
			$(this).css('color', 'rgb(from var(--bs-body-color) r g b / 35%)');
		}
		else {
			$(this).removeAttr('style');
		}
	});
}

function SetTooltips() {
	var tooltipTitles = $('[data-overflow-tooltip="true"]');
	$(tooltipTitles).each(function (i) {
		if (this.scrollWidth > this.offsetWidth) {
			$(this).tooltip();
		}
	});
	$('[data-action-tooltip="true"]').tooltip();
}

function HideAllTooltips() {
	$('.tooltip.show').tooltip('hide');
}

function SetUploadActive(uploadID, btnID) {
	$('#fileWaiting').prop('hidden', true);
	const supportingDocumentation = document.getElementById(uploadID)
	var UploadDocBtn = document.getElementById(btnID)
	if (supportingDocumentation.files.length > 0) {
		UploadDocBtn.disabled = false;
	}
	else {
		UploadDocBtn.disabled = true;
	}
}

function showFileWaiting() {
	$('#fileWaiting').prop('hidden', false);
}

function cancelFilePick(id) {
	$(`#${id}`).on('cancel', function () {
		$('#fileWaiting').prop('hidden', true);
	});
}

function disableSubmitBtns() {
	$('[data-disable-btn]').each(function () {
		var type = $(this).attr('data-disable-btn');
		var icon = $(this).attr('data-disable-btn-icon') || null;
		var text = $(this).attr('data-disable-btn-text') || null;
		$(this).on('click', function () {
			if (text != null && (text.includes("Submit") || text.includes("Submitting") || text.includes("Save") || text.includes("Saving"))) {
				if (isValid) {
					$(this).attr('disabled', true);
				}
			}
			else {
				$(this).attr('disabled', true);
			}
			switch (type) {
				case "htmlBtn":
					if (text.includes("Submit")) {
						if (isValid) {
							$(this).html(`<span class="fa-fade">${text}...</span>`);
						}
					}
					else {
						$(this).html(`<span class="fa-fade">${text}...</span>`);
					}
					break;
				case "htmlIconBtn":
					if (text.includes("Saving") || text.includes("Save")) {
						if (isValid) {
							$(this).html(`<span class="fa-fade"><span class="fas ${icon} fa-bounce"></span>&nbsp;${text}...</span>`);
						}
					}
					else {
						$(this).html(`<span class="fa-fade"><span class="fas ${icon} fa-bounce"></span>&nbsp;${text}...</span>`);
					}
					break;
				case "aspBtn":
					$(this).val(`${text}...`);
					$(this).addClass('fa-fade');
					break;
				case "aspIconBtn":
					$(this).addClass('fa-fade');
					break;
			}
		});
	});
}

function clickAspBtn(btnID) {
	var btn = document.getElementById(btnID);
	btn.click();
}

function enterBtn() {
	$('[data-enter-btn]').each(function () {
		$(this).on('keydown', function (event) {
			if (event.key === 'Enter') {
				var id = $(this).attr('data-enter-btn');
				const searchBtn = document.getElementById(id);
				searchBtn.click();
			}
		});
	});
}

function addSignatureEmails(addressID, btnID) {
	$(`#${addressID}`).on('change keyup', function () {
		var validEmail = $(`#${addressID}`).val().indexOf("@cwlp.com") > 1 || $(`#${addressID}`).val().indexOf("@springfield.il.us") > 1;
		if (validEmail && $(`#${addressID}`).val().length > 0) {
			$(`#${btnID}`).prop('disabled', false);
		}
		else {
			$(`#${btnID}`).prop('disabled', true);
		}
	});
}

function SetDeleteModal(ordID) {
	const hdnDeleteID = $('#hdnDeleteID');
	$(hdnDeleteID).attr("value", ordID);
	$('#deleteModal').modal('show');

}

function showDatePicker() {
	$('[data-calendar-btn]').each(function () {
		$(this).on('click', function (event) {
			var id = $(this).attr('data-calendar-btn');
			const dateInput = document.getElementById(id);
			dateInput.showPicker();
		});
	});
}