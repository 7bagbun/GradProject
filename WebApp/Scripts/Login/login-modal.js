$(document).ready(() => {
    $('.m-input').click(() => {
        $('#err-msg').addClass('hide');
    })

    $('#login-form').submit((e) => {
        e.preventDefault();

        var username = $('#username').val();
        var passwd = $('#passwd').val();
        var errMsgEle = $('#err-msg');

        if (checkEmpty(username) || checkEmpty(passwd)) {
            errMsgEle.text("Please fill out every field!");
            errMsgEle.removeClass('hide');
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/member/login',
            async: true,
            data: {
                username: username,
                passwd: passwd,
            },
            success: (data) => {
                if (data.result) {
                    location.reload();
                } else {
                    errMsgEle.text(data.msg);
                    errMsgEle.removeClass('hide');
                }
            },
            failure: (err) => {
                console.log(err);
            }
        });
    })
});

function checkEmpty(data) {
    if (data === '') {
        return true;
    } else {
        return false;
    }
}
