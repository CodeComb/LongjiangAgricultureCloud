var p = 0;
var lock = false;
var id = null

function Load()
{
    LoadInformations();
}

function LoadInformations()
{
    if ($('#lstInformations').length > 0)
    {
        if (lock) return;
        lock = true;
        $.get('/Mobile/MInformation/ListRaw', { p: p, id: id }, function (data) {
            if (data.length < 5) {
                $('#lstInformations').html('<div class="no-more">没有更多信息了！</div>');
            }
            $('#lstInformations').append(data);
            p++;
            lock = false;
        }); 
    }
}

$(document).ready(function () {
    Load();
});

$(window).scroll(function () {
    var totalheight = parseFloat($(window).height()) + parseFloat($(window).scrollTop());
    if ($(document).height() <= totalheight) {
        Load();
    }
});