function deleteComment(id) {
    swal({
        text: "你確定要刪除此評論嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/deleteComment", { commentId: id }, (data) => {
                if (data.isSucceed) {
                    location = "/admin/commentList"
                }
            });
        }
    });
}
