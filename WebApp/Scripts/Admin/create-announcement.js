const el = $("#time");
const displayTime = () => {
    const m = moment();
    let time = m.format("yyyy/MM/DD hh:mm:ss A");

    el.val(time);
}
displayTime();
setInterval(displayTime, 1000);

function submitForm() {
    if (!validForm()) {
        $("#msg").removeClass("hidden");
        return;
    }

    const formData = $("#create-form").serialize();
    $.post("/moderation/createAnnouncement", formData, (data) => {
        if (data.isSucceed) {
            window.location = "/admin/announcementList";
        }
    })
}

function validForm() {
    const title = $("#title").val();
    const content = $("#title").val();

    if (title === "" || content === "") {
        return false
    } else {
        return true;
    }
}

function hideMsg() {
    $("#msg").addClass("hidden");
}
