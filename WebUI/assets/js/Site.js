var isShown = false;
function showLoadingModal() {
	if (!isShown) {
		$('#loadingModal').modal('show');
		$('.modal-backdrop').css("opacity", ".75");
		isShown = true;
	}
	
}

function hideLoadingModal() {
	if (isShown) {
		$('#loadingModal').modal('hide').hide();
		$('#loadingModal').removeClass('show');
		$('body').removeClass('modal-open');
		$('.modal-backdrop').css('opacity', '');
		$('.modal-backdrop').remove();
		isShown = false;
	}
}

function appDevModalClick(modal) {
	$(modal).modal('hide');
	showLoadingModal();
}