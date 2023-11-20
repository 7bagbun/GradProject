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

function searchMember() {
    const query = $("#query").val().toLowerCase();
    const rows = $(".data-row");

    if (query === "") {
        rows.css("display", "table-row");
    } else {
        rows.each((i) => {
            let username = rows[i].getElementsByClassName("username")[0].innerText.toLowerCase();
            let email = rows[i].getElementsByClassName("email")[0].innerText.toLowerCase();

            if (username.indexOf(query) > -1 || email.indexOf(query) > -1) {
                rows[i].style.display = "";
            } else {
                rows[i].style.display = "none";
            }
        })
    }
}
