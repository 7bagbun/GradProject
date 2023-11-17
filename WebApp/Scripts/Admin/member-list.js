function suspendMember(id) {
    swal({
        text: "確定要停權此會員嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/setSuspend", { id: id, state: true }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

function unsuspendMember(id) {
    swal({
        text: "確定要取消停權此會員嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/setSuspend", { id: id, state: false }, (data) => {
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
