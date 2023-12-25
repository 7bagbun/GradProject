function setCtrl(state) {
    if (state) {
        $(".btn-edit").removeClass("hidden");
        $(".btn-show").addClass("hidden");
    } else {
        $(".btn-edit").addClass("hidden");
        $(".btn-show").removeClass("hidden");
    }

    $("#edit-form .input").attr("disabled", !state);
}

function submitChanges() {
    let formData = $("#edit-form").serialize();

    $.post("/moderation/editAnnouncement", formData, (data) => {
        if (data.isSucceed) {
            location.reload();
            setCtrl(false);
        }
    });
}