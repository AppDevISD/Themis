var searchActive = false;
var validEmails = ["@cwlp.com", "@springfield.il.us", "@lincolnlibrary.info"];
var pendingFiles = [];
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
	$('[data-action-tooltip="true"]').tooltip({
		delay: {"show": 1500, "hide": 0}
	});
}

function HideAllTooltips() {
	$('.tooltip.show').tooltip('hide');
}

function SetUploadActive(uploadID, btnID) {
	$('#fileWaiting').prop('hidden', true);
	const supportingDocumentation = document.getElementById(uploadID)
	var UploadDocBtn = document.getElementById(btnID)
	if (supportingDocumentation.files.length > 0) {
		pendingFiles = $(`#${uploadID}`)[0].files;
		UploadDocBtn.disabled = false;
	}
	else {
		UploadDocBtn.disabled = true;
		pendingFiles = [];
	}
}

function GetPendingFiles(uploadID, btnID) {
	var UploadDocBtn = document.getElementById(btnID)
	if (pendingFiles.length > 0) {
		$(`#${uploadID}`).prop('files', pendingFiles);
		UploadDocBtn.disabled = false;
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
		var select = $(this).attr('data-disable-btn-select') || null;
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
					if (select) {
						$(`#${select}`).addClass('fa-fade');
					}
					break;
			}
		});
	});
}

function clickAspBtn(btnID, clearPendingFiles) {
	var btn = document.getElementById(btnID);
	const clearPending = clearPendingFiles || false;
	btn.click();

	if (clearPendingFiles) {
		pendingFiles = [];
	}
}

function enterBtn() {
	$('[data-enter-btn]').each(function () {
		$(this).on('keydown', function (event) {
			if (event.key === 'Enter') {
				var id = $(this).attr('data-enter-btn');
				const searchBtn = document.getElementById(id);
				searchBtn.click();

				if (id.toLowerCase().includes("search")) {
					searchActive = true;
				}
			}
		});
	});
}

function addSignatureEmails(controls) {
	$(controls).each(function () {
		const cfg = this || {};
		const addressID = cfg.addressID;
		const btnID = cfg.btnID;

		$(`#${addressID}`).on('change keyup', function () {
			var validEmail = $(`#${addressID}`).val().toLowerCase().indexOf("@cwlp.com") > 1 || $(`#${addressID}`).val().toLowerCase().indexOf("@springfield.il.us") || $(`#${addressID}`).val().toLowerCase().indexOf("@lincolnlibrary.info") > 1;
			if (validEmail && $(`#${addressID}`).val().length > 0) {
				$(`#${btnID}`).prop('disabled', false);
			}
			else {
				$(`#${btnID}`).prop('disabled', true);
			}
		});
	});
	
}

function SetDeleteModal(ordID, showModal) {
	const hdnDeleteID = $('#hdnDeleteID');
	$(hdnDeleteID).attr("value", ordID);
	if (showModal) {
		$('#deleteModal').modal('show');
	}

}

function DeleteModalCancel(config) {
	const cfg = config || {};
	const btns = cfg.btnIDs;
	const hdnID = cfg.hdnID;

	$(btns).each(function () {
		$(`#${this}`).on('click', function () {
			$(`#${hdnID}`).val("");
		})
	});
	
}

function showDatePicker() {
	$('[data-calendar-btn]').each(function () {
		$(this).on('click', function () {
			var id = $(this).attr('data-calendar-btn');
			const dateInput = document.getElementById(id);
			dateInput.showPicker();
		});
	});
}

function SetModalDatePicker(modal) {
	document.addEventListener('DOMContentLoaded', function () {
		switch (modal) {
			case "signatureModal":
				const pickerBtn = document.getElementById('sigDatePicker');
				if (pickerBtn) {
					pickerBtn.addEventListener('click', function () {
						const input = document.getElementById('sigDate');
						if (input && typeof input.showPicker === 'function') {
							try {
								input.showPicker();
							} catch (e) {
								console.warn('showPicker error:', e);
							}
						}
					});
				}
				break;
		}
	});
}

function saveTabState() {
	$('[data-toggle="tab"]').each(function () {
		$(this).on('click', function () {
			let currentTabId = $(this).attr('id');
			let value = $('#hdnActiveTabs').val().split(',').map(v => v.trim()).filter(Boolean);

			let allTabIds = [];

			$(this).closest('.nav').children().each(function () {
				let tabId = $(this).children().attr('id');
				if (tabId) allTabIds.push(tabId);

				if ($(this).hasClass('dropdown')) {
					$(this).find('.dropdown-menu').children().each(function () {
						let dropdownTabId = $(this).attr('id');
						if (dropdownTabId) allTabIds.push(dropdownTabId);
					});
				}
			});

			value = value.filter(v => !allTabIds.includes(v));

			if (currentTabId && !value.includes(currentTabId)) {
				value.push(currentTabId);
				if ($(`#${currentTabId}`).parent().parent().hasClass('dropdown')) {
					let ddID = $(`#${currentTabId}`).parent().parent().find('.dropdown-toggle').attr('id');
					if (ddID && !value.includes(ddID)) {
						value.push(ddID);
					}
				}
			}

			$('#hdnActiveTabs').val(value.join(','));
		});
	});
}

function clearFilterBtn() {
	$('[data-clear-control]').each(function () {
		const clearBtn = $(this);
		var id = $(this).attr('data-clear-control');
		const control = $(`#${id}`);
		switch ($(control).val().length > 0) {
			case true:
				$(clearBtn).show();
				break;
			case false:
				$(clearBtn).hide();
				break;
		}
		control.on('change keydown', function () {
			switch ($(control).val().length > 0) {
				case true:
					clearBtn.show();
					break;
				case false:
					clearBtn.hide();
					break;
			}
		})
		$(this).on('click', function () {
			$(control).val('');
			$(clearBtn).hide();

			if ($(clearBtn).hasAttr('data-clear-unique')) {
				var postBackID = $(this).attr('data-clear-unique');
				__doPostBack(postBackID, '');
				showLoadingModal();
			}

			if ($(control).hasAttr('data-enter-btn') && searchActive) {
				var btnID = $(control).attr('data-enter-btn');
				const btn = document.getElementById(btnID);
				btn.click();

				searchActive = false;
			}
		});
	});
}