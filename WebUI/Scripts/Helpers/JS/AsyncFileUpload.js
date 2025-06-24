var pendingFiles = [];
const uploadedFiles = [];

function GetPendingFiles(uploadID, btnID) {
	var UploadDocBtn = document.getElementById(btnID)
	if (pendingFiles.length > 0) {
		$(`#${uploadID}`).prop('files', pendingFiles);
		UploadDocBtn.disabled = false;
	}
}

function asyncFileUpload(uploadID, uploadBtn) {
	const files = $(`#${uploadID}`)[0].files;
	if (files.length === 0) return alert("No files selected");

	const formData = new FormData();
	for (let i = 0; i < files.length; i++) {
		formData.append("uploadedFiles", files[i]);
	}

	$.ajax({
		url: "./Scripts/Helpers/CSharp/FileUploadHandler.ashx/UploadFile",
		type: "POST",
		data: formData,
		contentType: false,
		processData: false,
		success: function (data) {
			clickAspBtn(uploadBtn);
			pendingFiles = [];
		},
		error: function (xhr, status, error) {
			console.warn("Upload failed:", error);
		}
	});
}