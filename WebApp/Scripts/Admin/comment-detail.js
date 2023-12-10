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
                    location = "/admin/commentList"
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
