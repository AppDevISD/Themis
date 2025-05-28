function multiValidation() {
	window["IsValidationGroupMatch"] = function (control, validationGroup) {
		if ((typeof (validationGroup) == "undefined") || (validationGroup == null)) {
			return true;
		}
		var controlGroup = "";
		var isGroupContained = false;
		if (typeof (control.validationGroup) == "string") {
			controlGroup = control.validationGroup;
			var controlGroupArray = [];
			if (validationGroup.indexOf(",") > -1) {
				controlGroupArray = validationGroup.split(",");
			}
			for (var i = 0; i < controlGroupArray.length; i++) {
				if (controlGroupArray[i].trim() == controlGroup.trim()) {
					isGroupContained = true;
				}
			}
		}
		return (controlGroup == validationGroup || isGroupContained);
	}
}