function search() {
    let query = $("#search").val().toLowerCase();
    const msg = $("#msg");
    const tb = $("table");
    let rows = tb.children().children("tr");
    let matches = 0;
    let tdModel;

    for (let i = 1; i < rows.length - 1; i++) {
        tdModel = rows[i].children[0].innerText.toLowerCase();

        if (tdModel.indexOf(query) > -1) {
            matches++;
            rows[i].style.display = "";
        } else {
            rows[i].style.display = "none";
        }
    }

    if (!matches) {
        msg.css("display", "none")
    } else {
        msg.css("display", "flex")
    }
}

function search2() {
    let query = $("#search").val().toLowerCase();
    let rows = $(".data-row");
    const msg = $("#msg");
    let matches = 0;
    let model;

    for (let i = 0; i < rows.length; i++) {
        model = rows[i].children[0].innerText.toLowerCase();

        if (model.indexOf(query) > -1) {
            matches++;
            rows[i].style.display = "flex";
        } else {
            rows[i].style.display = "none";
        }

        if (!matches) {
            msg.css("display", "flex") //not found
        } else {
            msg.css("display", "none") //found
        }
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
