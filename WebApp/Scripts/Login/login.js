$(document).ready(() => {
    $("input").on("click", () => {
        $("#err-msg").addClass("hidden");
    })

    $("#page-login").on("submit", e => {
        e.preventDefault();

        let username = $("#username-page").val();
        let passwd = $("#passwd-page").val();
        let referer = $("#referer").val();
        const errMsgEle = $("#msg");

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
                referer: referer
            },
            success: (data) => {
                if (data.admin) {
                    window.location = data.redirUrl;
                    return;
                }

                if (data.result) {
                    if (data.referer === "") {
                        location = "/";
                    } else {
                        location = data.referer;
                    }
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