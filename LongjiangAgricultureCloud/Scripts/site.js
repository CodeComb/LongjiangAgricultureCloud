$(document).ready(function () {
    $('.datetime').datetimepicker();
});

function postDelete(url, id) {
    $.post(url, { _csrf: csrf }, function (data) {
        $('#' + id).remove();
        if (data == 'ok' || data == 'OK')
            ;
        else
            popMsg(data);
        closeDialog();
    });
}

function deleteDialog(url, id) {
    var html = '<div class="message-bg"></div><div class="message-outer"><div class="message-container">' +
        '<h3>提示信息</h3>' +
        '<p>点击删除按钮后，该记录将被永久删除，您确定要这样做吗？</p>' +
        '<div class="message-buttons"><a href="javascript:postDelete(\'' + url + '\', \'' + id + '\')" class="btn red">删除</a> <a href="javascript:closeDialog()" class="btn">取消</a></div>' +
        '</div></div>';
    var dom = $(html);
    $('body').append(dom);
    $(".message-outer").css('margin-top', -($(".message-outer").outerHeight() / 2));
    setTimeout(function () { $(".message-bg").addClass('active'); $(".message-outer").addClass('active'); }, 10);
}

function closeDialog() {
    $('.message-bg').removeClass('active');
    $('.message-outer').removeClass('active');
    setTimeout(function () {
        $('.message-bg').remove();
        $('.message-outer').remove();
    }, 200);
}