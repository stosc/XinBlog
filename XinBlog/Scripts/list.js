$(document).ready(function () {
    $('#next').on('click', function () {
        getPage(true);
    });
    $('#pre').on('click', function () {
        getPage(false);
    });
    loadHotArticle();
});

function getPage(isAdd)
{
    var pageIndex = $('#pageIndex').val();
    var pi = Number(pageIndex);
    if (isAdd)
        pi++
    else
        pi--;
    $('#pageIndex').val(pi);
    $('#list').submit();
}



function loadHotArticle()
{
    html = "";
    $.ajax({
        type: "get",
        url: "/home/HotArticle",
        dataType: 'json',
        contentType: 'application/json',
        success: function (arts) {
            if (arts.length > 0)
            {
                arts.forEach(function (e) {
                    html += ' <li><a href="/home/article/' + e.id + '" title="' + e.Title + '" target="_blank">' + e.Title + '</a></li>';
                });
                $('#hotList').html(html);
            }

        }
    });
}