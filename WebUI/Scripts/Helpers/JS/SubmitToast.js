const getStoredToast = () => localStorage.getItem('showToast');

function pageLoad(sender, args) {
	GetToastStatus();
	ToastAnimationHelper();
}

$(document).ready(function () {
	if (getStoredToast() == 'show') {
		try {
			$('#submitToast').toast('show');
			localStorage.setItem('showToast', 'hide');
		} catch (e) { }
	}
	ToastAnimationHelper();
});

function ShowSubmitToast() {
	localStorage.setItem('showToast', 'show');
}

function ShowEmailToast(status) {
	console.log(status);
	switch (status) {

		case "success":
			$('#submitToast').removeClass("text-bg-danger");
			$('#submitToast').addClass("text-bg-success");
			$('#toastMessage').html("Email Sent!");
			break;
		case "failed":
			$('#submitToast').removeClass("text-bg-success");
			$('#submitToast').addClass("text-bg-danger");
			$('#toastMessage').html("Something went wrong sending the email!");
			break;
	}

	ShowSubmitToast();
	GetToastStatus();
}

function GetToastStatus() {
	if (getStoredToast() == 'show') {
		try {
			$('#submitToast').toast('show');
			localStorage.setItem('showToast', 'hide');
		} catch (e) { }
	}
}

function ToastAnimationHelper() {
	$(document).on('hide.bs.toast', function () {
		$('#submitToast').addClass("hide");
	})
	$(document).on('hidden.bs.toast', function () {
		setTimeout(function () { $("#submitToast").removeClass("hide") }, 1000);
	});
}