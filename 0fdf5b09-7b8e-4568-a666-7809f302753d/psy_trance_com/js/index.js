$(function () {
    $(".search").focus();
    var search = $(".search").val();

    if (window.location.hash === "#/genres/") {
        
        $.get("/api/genres/").success(function (data) {
            $.each(data, function (i, data) {
                showGenres(data);
            });
        });

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
    }
    else if (window.location.hash === "#/artists/") {
        $.get("/api/artists/").success(function (data) {
            $.each(data, function (i, data) {
                showArtists(data);
            });
        });

        $(".search").keyup(function () {
            if (search !== $(".search").val()) {
                $.get("/api/artists/?artistName=" + $(".search").val()).success(function (data) {
                    $("div.artists").empty();

                    $.each(data, function (i, data) {
                        showArtists(data);
                    });
                });

                search = $(".search").val();
            }
        });
    }
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
        }).children("div:last-child").append("<div><a href=/#/" + Genre.Id + ">" + Genre.Name + "</a><br/>Исполнителей: " + Genre.ArtistsCount + "<br/>Альбомов: " + Genre.AlbumsCount + "<br /></div>");

};

var showArtists = function (Artist) {
    var letter = Artist.Name.substr(0, 1).toUpperCase();

    if (!$("div.artists").children().filter(
        function () {
            return $(this).children("div:first-child").text() === letter;
    }).length) {
        $("div.artists").append("<div><div>" + letter + "</div><div></div></div>");
    }

    $("div.artists").children().filter(
        function () {
            return $(this).children("div:first-child").text() === letter;
        }).children("div:last-child").append("<div><a href=/#/" + Artist.Id + ">" + Artist.Name + "</a><br/>Альбомов: " + Artist.AlbumsCount + "<br /></div>");

};
