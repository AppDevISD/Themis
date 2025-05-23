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