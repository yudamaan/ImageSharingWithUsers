$(function () {
    $("#like-button").click(function () {
        var userId = $("#user").data("user-id");
        var imageId = $("#image").data("image-id");
        $("#like-button").hide();
        $.post("/home/like", { userId: userId, imageId: imageId }, function (result) {
            $("#likes").html(result.LikesCount + " Likes");
        });
    });
});