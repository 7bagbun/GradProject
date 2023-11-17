function suspendMember(id, state = true) {
    swal({
        text: "你確定要停權此會員嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/setSuspend", { id: id, state: state }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

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
            $.post("/moderation/deleteComment", { id: id }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}
