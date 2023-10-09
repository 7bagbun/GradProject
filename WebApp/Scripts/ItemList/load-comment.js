$("document").ready(() => {
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
    section.after(`
        <div id="comment-template" class="container comment">
        <div class="row"><div class="col-1"><img class="profile-picture" /></div>
        <div class="col list-group"><div class="row list-group-item comment-header">
        <div class="col"><span class="comment-author"></span><span class="comment-date"></span>
        </div><div class="col comment-stars"></div><div class="col comment-btn">
        <button class="tool-btn"><i class="fa fa-ellipsis-h"></i></button>
        </div> </div> <div class="row list-group-item comment-body"><div class="col comment-content">
        </div></div></div></div></div>`);

    for (let i = 0; i < comments.length; i++) {
        var comment = $("#comment-template");
        comment.find(".comment-author").text(comments[i].Author);
        comment.find(".comment-date").text(`${comments[i].CreatedAt}`);
        comment.find(".comment-stars").html(placeStars(comments[i].Rating));
        comment.find(".comment-content").text(comments[i].Content);
        comment.find(".profile-picture").attr("src", "/member/getpfpbyusername?username=" + comments[i].Author)
        comment.attr("id", "");
        comment.clone().appendTo("#comments");
    }

    comment.remove();
}
function placeStars(rating) {
    let html = "";

    for (let i = 1; i <= 5; i++) {
        if (i <= rating) {
            html += '<span class="fa fa-star colored-star"></span>';
        } else {
            html += '<span class="fa fa-star uncolored-star"></span>';
        }
    }

    return html;
}