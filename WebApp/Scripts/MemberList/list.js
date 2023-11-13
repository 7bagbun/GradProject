function search() {
    let query = $("#search").val().toLowerCase();
    const tb = $("table");
    let rows = tb.children().children("tr");
    let matches = 0;
    let tdName, tdUsrname;

    for (let i = 1; i < rows.length - 1; i++) {
        tdName = rows[i].children[0].innerText.toLowerCase();
        tdUsrname = rows[i].children[1].innerText.toLowerCase();

        if (tdName.indexOf(query) > -1 || tdUsrname.indexOf(query) > -1) {
            matches++;
            rows[i].style.display = "";
        } else {
            rows[i].style.display = "none";
        }
    }

    if (!matches) {
        $(".empty-show").css("display", "table-row");
        $(".empty-hide").css("display", "none");
    } else {
        $(".empty-show").css("display", "none");
        $(".empty-hide").css("display", "table-row");
    }
}

function bulkDelete() {
    let selected = $("input[type='checkbox']:checked");
    let id = [];

    if (!selected.length) {
        return;
    }

    for (let i = 0; i < selected.length; i++) {
        id.push(selected[i].value);
    }

    if (confirm(`Are you sure you want to delete ${id.length} item(s)?`)) {
        $.ajax({
            type: "POST",
            url: "/Home/BulkDeleteMemberAjax",
            async: true,
            data: {
                ids: id
            },
            success: (data) => {
                if (data.result) {
                    window.location.href = "./MemberList";
                } else {
                    console.log(data.errMsg);
                }
            },
            error: (err) => {
                console.log(err);
            }
        });
    }
}