var identity

$.get("/identity/getidentity", (data) => {
    identity = data.identity;
});

function updateCount(e) {
    let count = e.value.length;
    const text = $("#text-count");
    text.text(count);

    if (count > 500) {
        text.css("color", "red");
        $("#btn-post").prop("disabled", true);
    } else {
        text.text(count);
        text.css("color", "#777");

        $.get("/identity/getidentity", (data) => {
            if (identity == "member") {
                $("#btn-post").prop("disabled", false);
            }
        });
    }
}

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

    const comment = $("#comment-template");

    for (let i = 0; i < comments.length; i++) {
        comment.removeAttr("id");
        if (comments[i].IsAuthor) {
            comment.find(".comment-header").addClass("comment-owner");
        } else {
            comment.find(".comment-header").removeClass("comment-owner");
        }

        comment.find(".comment-author").text(comments[i].Author);
        comment.find(".comment-date").text(comments[i].CreatedAt);
        comment.find(".comment-stars").html(placeStars(comments[i].Rating));
        comment.find(".comment-content").text(comments[i].Content);
        comment.find(".profile-picture")
            .attr("src", "/member/getpfpbyusername?username=" + comments[i].Author)
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