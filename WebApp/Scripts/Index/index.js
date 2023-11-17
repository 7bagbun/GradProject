$('.owl-carousel').owlCarousel({
    loop: true,
    margin: 10,
    dots: true,
    autoplay: true,
    autoplayHoverPause: true,
    responsive: {
        0: {
            items: 1
        }
    },
})

var newsJson;

$.ajax({
    type: "GET",
    url: "/news/getallnews",
    success: (data) => {
        newsJson = data;

        if (Object.keys(newsJson).length) {
            displayNewsDetail(0);
        }

        displayNews(newsJson);
    },
    error: (err) => {
        console.log(err);
    }
});


function displayNews(news) {
    const newsEles = $(".news-item-empty");

    for (let i = 0; i < 4; i++) {
        if (i >= Object.keys(news).length) {
            return;
        }

        let el = newsEles.eq(i);
        el.removeClass("news-item-empty");

        let days = (new Date().getTime() - new Date(news[i].CreatedDate).getTime()) / 86400000;

        if (days >= 365) {
            days = Math.floor(days / 365);
            days = days + "年前";
        }
        if (days >= 30) {
            days = Math.floor(days / 30);
            days = days + "個月前";
        }
        if (days >= 1) {
            days = Math.floor(days);
            days = days + "天前";
        } else {
            days = "今天";
        }

        if (news[i].Type) {
            el.addClass("news-item-danger");
        }

        el.attr("onclick", `displayNewsDetail(${i})`);
        el.find("small").text(days);
        el.find("h5").text(news[i].Title);
        el.find("p").text(news[i].Content);
    }
}

function displayNewsDetail(i) {
    const el = $("#news-detail");
    let type;
    switch (newsJson[i].Type) {
        case 0:
            type = "一般通知";
            break;
        case 1:
            type = "重要公告";
            break;
        default:
            break;
    }

    el.find(".news-title").text(newsJson[i].Title);
    el.find(".news-date").text(newsJson[i].CreatedDate);
    el.find(".news-type").text(type);
    el.find(".news-content").text(newsJson[i].Content);
}
