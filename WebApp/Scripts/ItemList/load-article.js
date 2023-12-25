function loadArticles() {
    $.get("/article/getById", { productId: id }, (data) => {
        placeArticles(data);
    });
}

function placeArticles(articles) {
    const temp = $("#article-template");

    for (let i = 0; i < articles.length; i++) {
        let artEle = temp.clone();
        artEle.removeAttr("id");
        artEle.removeClass("hidden");
        artEle.find(".comment-author").text(articles[i].title).attr("href", articles[i].link);
        artEle.find(".comment-content").text(articles[i].content);
        artEle.find(".profile-picture")
            .attr("src", articles[i].image);
        artEle.appendTo("#articles");
    }

    temp.remove();
}
