function FileSaveOnPostback(file) {
	$.ajax({
		type: "POST",
		async: false,
		url: "./Scripts/Helpers/CSharp/FileUploadSaving.asmx/SaveFileOnPostback",
		contentType: "application/json",
		dataType: "json"
	});
}