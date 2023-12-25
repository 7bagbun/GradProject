$(document).ready(() => { updateBtnText(id) })

function track(pid) {
    $.ajax({
        type: "POST",
        url: "/trackItem/trackAjax",
        async: true,
        data: {
            productId: pid
        },
        success: (data) => {
            if (data.isSucceed) {
                updateBtnText(pid);
            } else {
                window.location = "/account/loginPage?referer=" + data.referer;
            }
        },
        error: (err) => {
            console.log(err);
        }
    })
}

function updateBtnText(pid) {
    $.ajax({
        type: "GET",
        url: `/trackitem/trackstatusajax?productId=${pid}`,
        async: true,
        success: (data) => {
            const btn = $("#follow");

            if (data.isTracked) {
                btn.text(" 已追蹤");
                btn.attr("class", "fas fa-heart unfollow");
            } else {
                btn.text(" 追蹤");
                btn.attr("class", "far fa-heart follow");
            }
        },
        error: (err) => {
            console.log(err);
        }
    })
}