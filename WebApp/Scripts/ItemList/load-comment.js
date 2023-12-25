var identity

$.get("/identity/getidentity", (data) => {
    identity = data.identity;

    if (identity == "member") {
        $("#content-area").prop("disabled", false);
    }
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

function loadComments(e) {
    $.ajax({
        type: "GET",
        url: "/comment/getbyid/" + e.data,
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
    const temp = $("#comment-template");

    for (let i = 0; i < comments.length; i++) {
        let commEle = temp.clone();
        commEle.removeAttr("id").removeClass("hidden");
        if (comments[i].IsAuthor) {
            commEle.find(".comment-header").addClass("comment-owner");
        } else {
            commEle.find(".comment-header").removeClass("comment-owner");
        }

        commEle.find(".comment-author").text(comments[i].Author);
        commEle.find(".comment-date").text(comments[i].CreatedAt);
        commEle.find(".comment-stars").html(placeStars(comments[i].Rating));
        commEle.find(".comment-content").text(comments[i].Content);
        commEle.find(".profile-picture")
            .attr("src", "/member/getpfpbyusername?username=" + comments[i].Author);

        commEle.appendTo("#comments");
    }

    temp.remove();
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