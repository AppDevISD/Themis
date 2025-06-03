$(document).ready(function () {
	FormatForms();
});
function FormatForms() {
	$("[data-type='telephone']").mask('(000) 000-0000');
	$("[data-type='extension']").mask('x000000');
	$("[data-type='currency']").each(function () {
		$(this).on("change keyup paste", function () {
			formatCurrency($(this));
		});

		$(this).on("focusout", function () {
			formatCurrency($(this), "blur");
		});
	});
	$('[type="date"]').each(function () {
		if ($(this).val() == "") {
			$(this).css('color', 'rgba(var(--bs-body-color-rgb), 0.25) !important');
		}
		else {
			$(this).css('color', 'var(--bs-body-color)');
		}
	});
	$('[type="date"]').on('change keyup', function () {
		if ($(this).val() == "") {
			$(this).css('color', 'rgba(var(--bs-body-color-rgb), 0.25) !important');
		}
		else {
			$(this).css('color', 'var(--bs-body-color)');
		}
	});

	var datePeriodStartEntered;
	var datePeriodEndEntered;
	let dateTerm = $("input[data-type='dateTerm']");
	let datePeriodStart = $("input[data-type='datePeriodStart']");
	let datePeriodEnd = $("input[data-type='datePeriodEnd']");

	LoadTermVars();
	function LoadTermVars() {
		if (datePeriodStart.val() != "") {
			datePeriodStartEntered = true;
		}
		else {
			datePeriodStartEntered = false;
		}
		if (datePeriodEnd.val() != "") {
			datePeriodEndEntered = true;
		}
		else {
			datePeriodEndEntered = false;
		}
		if (datePeriodStart.val() != "" && datePeriodEnd.val() != "") {
			GetTermDate();
		}
	}
	datePeriodStart.each(function () {
		$(this).on("change keyup paste", function () {
			let output = "";
			if (datePeriodStartEntered && datePeriodEndEntered && $(this).val() != "") {
				GetTermDate();
				return output;
			}
			else if (datePeriodEndEntered && $(this).val() != "") {
				datePeriodStartEntered = true;
				GetTermDate();
				return output;
			}
			else if ($(this).val() == "") {
				datePeriodStartEntered = false;
				dateTerm.val(output);
				return output;
			}
			else {
				datePeriodStartEntered = true;
				dateTerm.val(output);
				return output;
			}
		});
	});
	datePeriodEnd.each(function () {
		$(this).on("change keyup paste", function () {
			let output = "";
			if (datePeriodStartEntered && datePeriodEndEntered && $(this).val() != "") {
				GetTermDate();
				return output;
			}
			else if (datePeriodStartEntered && $(this).val() != "") {
				datePeriodEndEntered = true;
				GetTermDate();
				return output;
			}
			else if ($(this).val() == "") {
				datePeriodEndEntered = false;
				dateTerm.val(output);
				return output;
			}
			else {
				datePeriodEndEntered = true;
				dateTerm.val(output);
				return output;
			}
		});
	});
	function GetTermDate() {
		let startDate = new Date(datePeriodStart.val());
		let endDate = new Date(datePeriodEnd.val());
		let TimeDifference = endDate.getTime() - startDate.getTime();
		let DaysDifference = Math.round(TimeDifference / (1000 * 3600 * 24));
		if (DaysDifference > 1 || DaysDifference == 0) {
			dateTerm.val(`${DaysDifference} Days`);
		}
		else {
			dateTerm.val(`${DaysDifference} Day`);
		}
	}

	var excludeTab = ["button", "radio", "checkbox", "file"];

	$('#ordView input, #ordView select, #ordView textarea').each(function () {
		var type = $(this).attr('type');
		var isExcludedType = $.inArray(type, excludeTab) !== -1;
		var isReadonly = $(this).attr('readonly') !== undefined;

		if (!isExcludedType && !isReadonly) {
			if ($('#ordView').attr('readonly') !== undefined) {
				$(this).attr('tabindex', '-1'); // disable tabbing
			} else {
				$(this).removeAttr('tabindex'); // re-enable tabbing
			}
		}
	});
}
function CurrencyFormatting() {
	$("[data-type='currency']").each(function () {
		formatCurrency($(this), "blur");
	});
}
function formatNumber(n) {
	return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
}
function formatCurrency(input, blur) {
	var input_val = input.val();
	if (input_val === "") { return; }

	var original_len = input_val.length;

	if (input_val.indexOf(".") >= 0) {
		var decimal_pos = input_val.indexOf(".");
		var left_side = input_val.substring(0, decimal_pos);
		var right_side = input_val.substring(decimal_pos);
		left_side = formatNumber(left_side);
		right_side = formatNumber(right_side);
		if (blur === "blur") {
			right_side += "00";
		}
		right_side = right_side.substring(0, 2);
		input_val = "$" + left_side + "." + right_side;

	} else {
		input_val = formatNumber(input_val);
		input_val = "$" + input_val;
		if (blur === "blur") {
			input_val += ".00";
		}
	}
	input.val(input_val);
}
function formatCurrencyDecimal(input, blur) {
	var input_val = input[0].innerHTML;
	if (input_val === "") { return; }

	var original_len = input_val.length;

	if (input_val.indexOf(".") >= 0) {
		var decimal_pos = input_val.indexOf(".");
		var left_side = input_val.substring(0, decimal_pos);
		var right_side = input_val.substring(decimal_pos);
		left_side = formatNumber(left_side);
		right_side = formatNumber(right_side);
		if (blur === "blur") {
			right_side += "00";
		}
		right_side = right_side.substring(0, 2);
		input_val = "$" + left_side + "." + right_side;

	} else {
		input_val = formatNumber(input_val);
		input_val = "$" + input_val;
		if (blur === "blur") {
			input_val += ".00";
		}
	}
	input[0].innerHTML = input_val;
}
function FormatAudit() {
	$("[data-type='Decimal']").each(function () {
		formatCurrencyDecimal($(this), "blur");
	});
}
function ValidationFormatting(Validators) {
	$(Validators).each(function () {
		var validator = this;
		var id = $(validator).attr('id').replace('Valid', '');
		var control;
		switch (validator.hasAttribute('data-table-validator')) {
			case true:
				control = $(`[data-validate=${id}]`);
				break;
			case false:
				control = $(`#${id}`);
				break;
		}
		var label = $(`[for=${id}]`);

		if (currentValidation.includes(validator.validationGroup)) {
			ValidatorValidate(validator);
		}
		switch (validator.isvalid) {

			case true:
				control.removeClass('invalid-border');
				label.removeClass('required-field');
				break;
			case false:
				control.addClass('invalid-border');
				label.addClass('required-field');
				break;
		}
		control.on('change', function () {

			ValidatorValidate(validator);
			switch (validator.isvalid) {

				case true:
					control.removeClass('invalid-border');
					label.removeClass('required-field');
					break;
				case false:
					control.addClass('invalid-border');
					label.addClass('required-field');
					break;
			}
		});

	});
}