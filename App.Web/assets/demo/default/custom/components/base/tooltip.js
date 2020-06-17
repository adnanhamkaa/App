jQuery(document).ready(function() {
	var itemToAppend = `<button type="button" class="btn btn-success" data-skin="dark" data-toggle="m-tooltip" data-placement="bottom" title="Dark skin">Dark skin</button>`
    $('#append').append(itemToAppend);

    var e = function(t) {
        var e = t.data("skin") ? "m-tooltip--skin-" + t.data("skin") : "",
            a = "auto" == t.data("width") ? "m-tooltop--auto-width" : "",
            n = t.data("trigger") ? t.data("trigger") : "hover";
        t.data("placement") && t.data("placement");
        t.tooltip({
            trigger: n,
            template: '<div class="m-tooltip ' + e + " " + a + ' tooltip" role="tooltip">                <div class="arrow"></div>                <div class="tooltip-inner"></div>            </div>'
        })
    },
    a = function() {
        $('[data-toggle="m-tooltip"]').each(function() {
            e($(this))
        })
    };

    a();
});
