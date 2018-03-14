$(document).ready(function () {
    /**
     * Sidebar Dropdown
     */
    $('.nav-dropdown-toggle').on('click', function (e) {
        e.preventDefault();
        $(this).parent().toggleClass('open');
    });

    // open sub-menu when an item is active.
    $('ul.nav').find('a.active').parent().parent().parent().addClass('open');

    /**
     * Sidebar Toggle
     */
    $('.sidebar-toggle').on('click', function (e) {
        e.preventDefault();
        $('body').toggleClass('sidebar-hidden');
    });

    /**
     * Mobile Sidebar Toggle
     */
    $('.sidebar-mobile-toggle').on('click', function () {
        $('body').toggleClass('sidebar-mobile-show');
    });

     
    $(".copybtn").click(function () {
        console.log("copybtn click");

        var card = $(this).parents(".col-card").clone();
        card.find('.card-actions').children().replaceWith('<span class="btn closebtn"><i class="icon-close"></i></span>');
        card.hide().insertAfter($(this).parents(".col-card")).fadeIn(500);

        resetname($(this).parents(".col-card"));
    });

    $("#cards").delegate(".closebtn", "click", function () {
        var e = $(this);

        e.parents(".col-card").fadeOut(500, function () {
            $(this).remove();

            var key = e.parents(".col-card").attr("key");
            $("#cards").find(".col-card[key='" + key + "']").each(function () {
                resetname($(this));
            });
        });
    });

    function resetname(d) {
        console.log("-------------resetname---------------");

        d.find(".form-control").each(function () {
            var objectname = $(this).attr("tag");
            console.log(objectname);
            $("#cards").find(".form-control[tag='" + objectname + "']").each(function (index) {
                var newname = $(this).attr("tag").replace("[]", "[" + index + "]");
                console.log(index + ":" + $(this).prop("name") + "--->" + newname);
                $(this).prop("name", newname);
            });
        });
    };
});
