const spinner = $("#spinner");
const div = $("#content");

$(document).ready(() => {
    loadProfileData(tab);
});

async function loadProfileData(type) {
    let url;

    toggleSpinner(true);
    changeDivContent("");

    switch (type) {
        case "track":
            window.history.replaceState(null, null, "?tab=" + type);
            url = "/account/profileTrackProduct";
            break;
        case "comment":
            window.history.replaceState(null, null, "?tab=" + type);
            url = "/account/profileComment";
            break;
        case "profile":
            window.history.replaceState(null, null, "?tab=" + type);
            url = "/account/profilePersonalData";
            break;
        default:
            window.history.replaceState(null, null, "?tab=track");
            url = "/account/profileTrackProduct";
            break;
    }

    $.get({
        url: url,
        success: (data) => {
            changeDivContent(data);
            updateActiveTab(type);

            if (type === "comment") {
                placeStars();
            } else if (type === "profile") {
                $("#input-pfp").on("change", () => {
                    togglePfpCtrls();
                    showPickedImage();
                });
            }
        }
    });

    toggleSpinner(false);
}

function changeDivContent(html) {
    div.html(html);
}

function updateActiveTab(tab) {
    $(".tab").removeClass("tab-active");
    $(`#tab-${tab}`).addClass("tab-active");
}

function placeStars() {
    let func = (i, val) => {
        let html = "";
        let rating = Number(val.innerText);
        for (let i = 1; i <= 5; i++) {
            if (i <= rating) {
                html += '<span class="fa fa-star colored-star"></span>';
            } else {
                html += '<span class="fa fa-star uncolored-star"></span>';
            }
        }

        val.innerHTML = html;
    };

    $.each($(".star-rating"), func);
}

function toggleSpinner(state) {
    if (state) {
        spinner.css("display", "block");
    } else {
        spinner.css("display", "none");
    }
}

function deleteComment(id) {
    swal({
        text: "你確定要刪除評論嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/comment/delete", { commentId: id }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

function untrackItem(id) {
    swal({
        text: "確定要取消追蹤嗎?",
        icon: "warning",
        buttons: {
            cancel: "取消",
            confirm: "確定"
        },
        dangerMode: true,
    }).then((decision) => {
        if (decision) {
            $.post("/trackItem/trackAjax", { productId: id }, (data) => {
                if (data.isSucceed) {
                    location.reload();
                }
            });
        }
    });
}

function toggleChangePassword() {
    $(".chpwd-trig").toggleClass("hidden");
    $(".chpwd-ctrl").toggleClass("hidden");
    $(".chpwd-ctrl input[type='password']").val("");
}

function submitPassword() {
    let oldPwd = $("#old-pwd").val();
    let newPwd = $("#new-pwd").val();

    if (oldPwd === "" || newPwd === "") {
        $("#msg-chpwd").text("請填兩個必要欄位").removeClass("hidden");
        return;
    }

    $.post("/account/changePassword", { oldPwd: oldPwd, newPwd: newPwd }, (data) => {
        if (data.isSucceed) {
            swal({
                title: "修改成功！",
                text: "下次登入時，請使用新的密碼。",
                icon: "info",
                buttons: {
                    confirm: "確定"
                }
            }).then(() => {
                toggleChangePassword();
                hideChpwdMsg();
            });
        } else {
            $("#msg-chpwd").text(data.msg).removeClass("hidden");
        }
    });
}

function hideChpwdMsg() {
    $("#msg-chpwd").addClass("hidden");
}

function showFilePicker() {
    $("#input-pfp").click();
}

function showPickedImage() {
    const file = $("#input-pfp");
    const reader = new FileReader();
    reader.onload = e => {
        let fileUrl = e.target.result;
        $("#pfp-display").attr("src", fileUrl).removeClass("hidden");
        $("#pfp-og-display").addClass("hidden");
    };

    reader.readAsDataURL(file.prop("files")[0]);
}

function togglePfpCtrls() {
    $(".btn-ctrl").toggleClass("hidden");
    $(".btn-edit").toggleClass("hidden");
    $("#pfp-og-display").toggleClass("hidden");
    $("#pfp-display").toggleClass("hidden");
}

function submitPfp() {
    let pfp = $("#input-pfp")[0].files[0];

    let formData = new FormData();
    formData.append("pfp", pfp);

    $.ajax({
        url: "/member/editPfp",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: () => {
            location.reload();
        },
        error: (err) => {
            console.log(err);
        }
    });
}
