$(document).ready(function () {
	const chartTabs = $('.jsChartTabs');
	const tabHeight = $(chartTabs[0]).height();
	const labelHeight = parseInt($('.codeCol').children('label').css('line-height'));
	const codeClips = $('.codeClip');

	for (var i = 0; i < chartTabs.length; i++) {
		var codePane = $(chartTabs[i]).children('.codePane');
		$(codePane).css({ 'height': `${tabHeight}px` });
	}

	for (var i = 0; i < codeClips.length; i++) {
		const maxHeight = tabHeight - labelHeight;
		$(codeClips[i]).css({ 'max-height': `${maxHeight}px` });
	}
});

$(window).on("resize", function () {
	const chartTabs = $('.jsChartTabs');
	const tabHeight = $(chartTabs[0]).height();
	const labelHeight = parseInt($('.codeCol').children('label').css('line-height'));
	const codeClips = $('.codeClip');

	for (var i = 0; i < chartTabs.length; i++) {
		var codePane = $(chartTabs[i]).children('.codePane');
		$(codePane).css({ 'height': `${tabHeight}px` });
	}

	for (var i = 0; i < codeClips.length; i++) {
		const maxHeight = tabHeight - labelHeight;
		$(codeClips[i]).css({ 'max-height': `${maxHeight}px` });
	}
});