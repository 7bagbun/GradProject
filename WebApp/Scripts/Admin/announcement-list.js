var newsJson;
var curr = 0;
var pages;

getNews();

function getNews(page = 1) {
    $.ajax({
        type: "GET",
        url: "/news/getAllNews?page=" + page,
        success: (data) => {
            newsJson = data;

            if (Object.keys(newsJson).length) {
                displayNewsDetail(0);
            }

            displayNews(newsJson);

            $.get("/news/getPageCount", null, (data) => {
                pages = data.pages
                setPagination(pages);
            });
        },
        error: (err) => {
            console.log(err);
        }
    });
}

function displayNews(news) {
    const newsEles = $(".news-item");

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
        } else if (days >= 30) {
            days = Math.floor(days / 30);
            days = days + "個月前";
        } else if (days >= 1) {
            days = Math.floor(days);
            days = days + "天前";
        } else {
            days = "今天";
        }

        if (news[i].Type) {
            el.addClass("news-item-danger");
        } else {
            el.removeClass("news-item-danger");
        }

        el.attr("onclick", `displayNewsDetail(${i})`);
        el.find("small").text(days);
        el.find("h5").text(news[i].Title);
        el.find("p").text(addLink(news[i].Content));
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
    el.find(".news-content").html(addLink(newsJson[i].Content));
}

function setPagination(pages) {
    if (curr < 3) {
        for (let i = 0; i < 3; i++) {
            if (i >= pages) {
                $(`#page-${i}`).addClass("disabled").attr("onclick", "");
                $(`#page-${i}>div`).text(i + 1);
            } else {
                $(`#page-${i}`).removeClass("disabled").attr("onclick", `changePage(${i})`);
                $(`#page-${i}>div`).text(i + 1).addClass("cursor-pointer");
            }
        }
    } else {
        let offset = Math.floor(curr / 3) * 3;
        for (let i = 0; i < 3; i++) {
            if (i + offset >= pages) {
                $(`#page-${i}`).addClass("disabled").attr("onclick", "");
                $(`#page-${i}>div`).text(i + 1 + offset);
            } else {
                $(`#page-${i}`).removeClass("disabled").attr("onclick", `changePage(${i + offset})`);
                $(`#page-${i}>div`).text(i + 1 + offset).addClass("cursor-pointer");
            }
        }
    }

    for (let i = 0; i < 3; i++) {
        $(`#page-${i}`).removeClass("active");
    }
    $(`#page-${curr % 3}`).addClass("active");

    if (pages > 3 && !(Math.ceil((curr + 1) / 3) === Math.ceil(pages / 3))) {
        $("#next").removeClass("disabled");
        $("#next>div").addClass("cursor-pointer");
    } else {
        $("#next").addClass("disabled");
        $("#next>div").removeClass("cursor-pointer");
    }

    if (curr < 3) {
        $("#prev").addClass("disabled");
        $("#prev>div").removeClass("cursor-pointer");
    } else {
        $("#prev").removeClass("disabled");
        $("#prev>div").addClass("cursor-pointer");
    }
}

function changeSection(direction) {
    if (direction) {
        curr = curr + 3;
        if (curr >= pages) {
            curr = pages - 1;
        }
    } else {
        curr = curr - 3;
        if (curr < 0) {
            curr = 0;
        }
    }

    getNews(curr + 1);
}

function changePage(pageNum) {
    curr = pageNum;
    getNews(curr + 1);
}

function deleteAnnouncement(id) {
    swal({
        text: "你確定要刪除此公告嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/moderation/deleteAnnouncement", { id: id }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

function addLink(news) {
    const re = /\[(.+)\]\((https:\/\/.+)\)/;
    return news.replace(re, "<a href='$2'>$1</a>").replace("\n", "<br>");
}
