function autoCollapse(id) {
	var newMenu = $(id);
	var dropdown = $(newMenu.attr('data-collapse-tabs'));
	var dropdownMenu = dropdown.children('.dropdown-menu');

	var children = newMenu.children(`:not(:last-child)`);
	var btns = children.children();
	var btnsCount = btns.length;
	$(btns[btnsCount - 1]).addClass('dropdown-item');
	$(btns[btnsCount - 1]).prependTo(dropdownMenu);
	var childrenCount = children.length;
	$(children[childrenCount - 1]).remove();

	if (dropdownMenu.children().length > 0) {
		dropdown.show();
	}
}
function autoExpand(id) {
	var newMenu = $(id);
	var dropdown = $(newMenu.attr('data-collapse-tabs'));
	var dropdownMenu = dropdown.children('.dropdown-menu');
	

	var btns = dropdownMenu.children();
	var collapsed = dropdownMenu.children();
	var li = `<li id='newElement' class='nav-item'></li>`;
	$(newMenu.children(':last-child')).before(li);
	var newElement = $('#newElement');
	$(collapsed[0]).removeClass('dropdown-item');
	$(collapsed[0]).prependTo(newElement);
	newElement.removeAttr('id');

	if (dropdownMenu.children().length === 0) {
		dropdown.hide();
	}
}
function CollapseTabs() {
	var collapseTabs = $('[data-collapse-tabs]');
	$(collapseTabs).each(function () {
		var id = `#${$(this).attr('id') }`;
		if (this.scrollWidth > this.offsetWidth) {
			while (this.scrollWidth > this.offsetWidth) {
				autoCollapse(id);
			}
			
		}
		else {
			autoExpand(id);
		}
	});

	$(window).on('resize', function () {
		$(collapseTabs).each(function () {
			var id = `#${$(this).attr('id')}`;
			if (this.scrollWidth > this.offsetWidth) {
				while (this.scrollWidth > this.offsetWidth) {
					autoCollapse(id);
				}
			}
			else {
				autoExpand(id);
			}
		});
	});
}