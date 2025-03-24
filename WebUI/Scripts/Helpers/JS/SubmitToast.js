const getStoredToast = () => localStorage.getItem('showToast');

function pageLoad(sender, args) {
	GetToastStatus();
}

$(document).ready(function () {
	if (getStoredToast() == 'show') {
		try {
			$('#submitToast').toast('show');
			localStorage.setItem('showToast', 'hide');
		} catch (e) { }
	}
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