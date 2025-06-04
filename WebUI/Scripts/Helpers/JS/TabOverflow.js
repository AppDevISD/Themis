var autocollapse = function (menu, maxHeight) {
    var tabsMenu = $(menu);
    

    tabsMenu.each(function () {
        var navHeight = $(this).innerHeight()
        var dropdown = $($(this).attr('data-collapse-tabs'));
        var dropdownMenu = dropdown.children('.dropdown-menu');

        if (navHeight > maxHeight) {

            while (navHeight > maxHeight) {
                var children = $(this).children(`:not(:last-child)`);
                var btns = children.children();
                var childrenCount = children.length;
                $(children[childrenCount - 1]).remove();
                var btnsCount = btns.length;
                $(btns[btnsCount - 1]).addClass('dropdown-item');
                $(btns[btnsCount - 1]).prependTo(dropdownMenu);
                navHeight = $(this).innerHeight();
            }
            dropdown.show();
        }
        else {
            while (navHeight < maxHeight && ($(this).children('li').length > 0) && dropdownMenu.children().length > 0) {
                var btns = dropdownMenu.children();
                var collapsed = dropdownMenu.children();
                var li = `<li id='newElement' class='nav-item'></li>`;
                $($(this).children(':last-child')).before(li);
                var newElement = $('#newElement');
                $(collapsed[0]).removeClass('dropdown-item');
                $(collapsed[0]).prependTo(newElement);
                newElement.removeAttr('id');
                navHeight = $(this).innerHeight();
			}
			if (dropdownMenu.children().length === 0) {
                dropdown.hide();
            }

            if (navHeight > maxHeight) {
                autocollapse('[data-collapse-tabs]', 50);
            }
        }
    });
};

function CollapseTabs() {
    autocollapse('[data-collapse-tabs]', 50);

    $(window).on('resize', function () {
        autocollapse('[data-collapse-tabs]', 50);
    });
}