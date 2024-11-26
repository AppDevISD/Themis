$(document).ready(function () {
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

    function formatNumber(n) {
        // format number 1000000 to 1,234,567
        return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
    }
    function formatCurrency(input, blur) {
        // appends $ to value, validates decimal side
        // and puts cursor back in right position.

        // get input value
        var input_val = input.val();

        // don't validate empty input
        if (input_val === "") { return; }

        // original length
        var original_len = input_val.length;

        // initial caret position 
        /*    var caret_pos = input.prop("selectionStart");*/

        // check for decimal
        if (input_val.indexOf(".") >= 0) {

            // get position of first decimal
            // this prevents multiple decimals from
            // being entered
            var decimal_pos = input_val.indexOf(".");

            // split number by decimal point
            var left_side = input_val.substring(0, decimal_pos);
            var right_side = input_val.substring(decimal_pos);

            // add commas to left side of number
            left_side = formatNumber(left_side);

            // validate right side
            right_side = formatNumber(right_side);

            // On blur make sure 2 numbers after decimal
            if (blur === "blur") {
                right_side += "00";
            }

            // Limit decimal to only 2 digits
            right_side = right_side.substring(0, 2);

            // join number by .
            input_val = "$" + left_side + "." + right_side;

        } else {
            // no decimal entered
            // add commas to number
            // remove all non-digits
            input_val = formatNumber(input_val);
            input_val = "$" + input_val;

            // final formatting
            if (blur === "blur") {
                input_val += ".00";
            }
        }

        // send updated string to input
        input.val(input_val);
    }

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
});



