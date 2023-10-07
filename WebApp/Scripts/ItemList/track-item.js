$(document).ready(() => { updateBtnText(id) })

function track(pid) {
    $.ajax({
        type: 'POST',
        url: '/trackitem/trackajax',
        async: true,
        data: {
            productId: pid
        },
        success: (data) => {
            if (data.isSucceed) {
                updateBtnText(pid);
            } else {
                window.location = data.redirUrl;
            }
        },
        failure: (err) => {
            console.log(err);
        }
    })
}

function updateBtnText(pid) {
    $.ajax({
        type: 'GET',
        url: `/trackitem/trackstatusajax?productId=${pid}`,
        async: true,
        success: (data) => {
            const btn = $("#follow");

            if (data.isTracked) {
                btn.text(" 已追蹤");
                btn.attr("class", "fa fa-heart unfollow");
            } else {
                btn.text(" 追蹤");
                btn.attr("class", "fa fa-heart-o follow");
            }
        },
        failure: (err) => {
            console.log(err);
        }
    })
}