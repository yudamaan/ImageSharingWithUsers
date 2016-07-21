$(function () {
    setInterval(function () {
        $.get("/home/views", function (result) {
            result.Views.forEach(function (view) {
                $(".views").each(function () {
                    if ($(this).data("image-id") == view.Id) {
                        $(this).html(view.Views+" Views")
                    }
                });
            });
        });
    }, 1000);
});
