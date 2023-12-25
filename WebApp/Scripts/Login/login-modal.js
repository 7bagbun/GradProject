$(document).ready(() => {
    $(".m-input").click(() => {
        $("#err-msg").addClass("hidden");
    })

    $("#login-form").submit((e) => {
        e.preventDefault();

        let username = $("#username").val();
        let passwd = $("#passwd").val();
        const errMsgEle = $("#err-msg");

        if (checkEmpty(username) || checkEmpty(passwd)) {
            errMsgEle.text("請確實輸入帳號與密碼");
            errMsgEle.removeClass("hidden");
            return;
        }

        $.ajax({
            type: "POST",
            url: "/account/login",
            async: true,
            data: {
                username: username,
                passwd: passwd,
            },
            success: (data) => {
                if (data.admin) {
                    window.location = data.redirUrl;
                    return;
                }

                if (data.result) {
                    location.reload();
                } else {
                    errMsgEle.text(data.msg);
                    errMsgEle.removeClass("hidden");
                }
            },
            error: (err) => {
                console.log(err);
            }
        });
    })
});

function checkEmpty(data) {
    if (data === "") {
        return true;
    } else {
        return false;
    }
}
