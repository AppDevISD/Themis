const getStoredToast = () => localStorage.getItem('showToast');
$(document).ready(function () {
	if (getStoredToast() == 'show') {
		try {
			$('#submitToast').toast('show');
			localStorage.setItem('showToast', '');
		} catch (e) { }
	}
});
function ShowSubmitToast() {
	localStorage.setItem('showToast', 'show');
}