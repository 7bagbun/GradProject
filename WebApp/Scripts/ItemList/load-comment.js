$("document").ready(() => {
    if ($("input[name='author']").attr("value") == undefined) {
        $("#btn-post").prop("disabled", true);
        $("#btn-post").prop("value", "登入後即可發布評論");
    }

    $("#post-form").submit(e => {
        e.preventDefault();
        const form = $("#post-form");
        const id = $("input[name='product']").attr("value");

        $.ajax({
            type: "POST",
            url: "/comment/create",
            data: form.serialize(),
            success: (data) => {
                if (data.Succeed) {
                    form[0].reset();
                    loadComments(id);
                }
            },
            fail: (err) => {
                console.log(err);
            }
        });
    })
});

function loadComments(id) {
    $.ajax({
            type: "GET",
            url: "/comment/getbyid/" + id,
            async: true,
            success: (data) => {
                placeComments(data);
            },
            failure: (err) => {
                console.log(err);
            }
    });
}

function placeComments(comments) {
    const section = $("#comments");
    section.empty();

    for (let i = 0; i < comments.length; i++) {
        let html =
        `<div class="comment">
            <div class="row comment-header">
                <div class="col">${comments[i].Author}</div>
                <div id="star" class="col">${placeStars(comments[i].Rating)}</div>
                <div class="col date">${comments[i].CreatedAt}</div>
            </div>
            <div class="row comment-body">
                <div class="col">${comments[i].Content}</div>
            </div>
        </div>`
        section.append(html);
    }
}
function placeStars(rating) {
    html = "";

    for (let i = 1; i <= 5; i++) {
        if (i <= rating) {
            html += '<span class="fa fa-star colored-star"></span>';
        } else {
            html += '<span class="fa fa-star uncolored-star"></span>';
        }
    }

    return html;
}