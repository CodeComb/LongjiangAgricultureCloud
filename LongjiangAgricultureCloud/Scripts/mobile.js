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

var EarthRadiusKm = 6378.137;

function Distance(dis)
{
    if (dis > 1000)
        return (dis / 1000).toFixed(1) + "公里";
    else
        return dis + '米';
}

function getDistance(p1Lat, p1Lng, p2Lat, p2Lng) {
    var dLat1InRad = p1Lat * (Math.PI / 180);
    var dLong1InRad = p1Lng * (Math.PI / 180);
    var dLat2InRad = p2Lat * (Math.PI / 180);
    var dLong2InRad = p2Lng * (Math.PI / 180);
    var dLongitude = dLong2InRad - dLong1InRad;
    var dLatitude = dLat2InRad - dLat1InRad;
    var a = Math.pow(Math.sin(dLatitude / 2), 2) + Math.cos(dLat1InRad) * Math.cos(dLat2InRad) * Math.pow(Math.sin(dLongitude / 2), 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var dDistance = EarthRadiusKm * c;
    return dDistance * 1000;
}