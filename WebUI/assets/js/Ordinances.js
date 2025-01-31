//function OrdinanceVisibility(fadeOut) {
//	var dataString = JSON.stringify({ fadeOut: fadeOut });
//	$.ajax({
//		type: "POST",
//		async: false,
//		url: "./Pages/OrdinanceTracking/Ordinances.aspx/OrdVisibility",
//		data: dataString,
//		contentType: "application/json",
//		dataType: "json"
//	});
//}
//function OrdTableFadeOut() {
//	var ordTable = document.getElementById('<%= ordTable.ClientID %>')
//	var ordView = document.getElementById('<%= ordView.ClientID %>')
//	$("[data-type='currency']").each(function () {
//		formatCurrency($(this), "blur");
//	});
//	$(ordTable).fadeOut(500);
//	setTimeout(() => {
//		$(ordView).fadeIn(500);
//	}, 500);
//	setTimeout(() => {
//		OrdinanceVisibility("table");
//	}, 1000);
//}
//function OrdTableFadeIn() {
//	var ordTable = document.getElementById('<%= ordTable.ClientID %>')
//	var ordView = document.getElementById('<%= ordView.ClientID %>')
//	$(ordView).fadeOut(500);
//	setTimeout(() => {
//		$(ordTable).fadeIn(500);
//	}, 500);
//	setTimeout(() => {
//		OrdinanceVisibility("ord");
//	}, 1000);
//}