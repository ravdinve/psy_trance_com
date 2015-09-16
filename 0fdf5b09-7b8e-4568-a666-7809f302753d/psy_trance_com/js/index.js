$(function () {
    $.get("/api/genres/").success(function (data) {
        $.each(data, function (i, data) {
            showGenres(data);
        });
    });

    var search = $(".search").val();

    $(".search").keyup(function () {
        if (search !== $(".search").val()) {
            $.get("/api/genres/?genreName=" + $(".search").val()).success(function (data) {
                $("div.genres").empty();

                $.each(data, function (i, data) {
                    showGenres(data);
                });
            });

            search = $(".search").val();
        }
        
    });
});
var showGenres = function (Genre) {
    var letter = Genre.Name.substr(0, 1).toUpperCase();

    if (!$("div.genres").children().filter(
        function () {
            return $(this).children("div:first-child").text() === letter;
        }).length) {
        $("div.genres").append("<div><div>" + letter + "</div><div></div></div>");
    }

    $("div.genres").children().filter(
        function() {
            return $(this).children("div:first-child").text() === letter;
        }).children("div:last-child").append("<div><a href=/#/" + Genre.Id + ">" + Genre.Name + "</a><br/>Исполнителей: " + Genre.Artists + "<br/>Альбомов: " + Genre.Albums + "<br /></div>");

};
