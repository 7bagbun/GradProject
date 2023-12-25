function hideComment(id) {
    swal({
        text: "你確定要隱藏此評論嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/hideComment", { commentId: id }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

function showComment(id) {
    swal({
        text: "你確定要解除隱藏此評論嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/showComment", { commentId: id }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

function searchComment() {
    const query = $("#query").val().toLowerCase();
    const rows = $(".data-row");

    if (query === "") {
        rows.css("display", "table-row");
    } else {
        rows.each((i) => {
            let username = rows[i].getElementsByClassName("username")[0].innerText.toLowerCase();
            let product = rows[i].getElementsByClassName("product")[0].innerText.toLowerCase();

            if (username.indexOf(query) > -1 || product.indexOf(query) > -1) {
                rows[i].style.display = "";
            } else {
                rows[i].style.display = "none";
            }
        })
    }
}
