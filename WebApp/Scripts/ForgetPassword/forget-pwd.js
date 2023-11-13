$(document).ready(() => {
    $("#btn-submit").on("click", submitForm);
});

function submitForm(e) {
    e.preventDefault();
    const form = $("#fp");
    const msg = $("#msg");
    const input = $("#email");
    msg.addClass("hidden");

    if (input.val() === "") {
        msg.text("請輸入電子信箱");
        msg.removeClass("hidden");
        return;
    }

    msg.text("處理中...");
    msg.removeClass("hidden");

    $.ajax({
        type: "POST",
        url: "/forgetpassword/sendemailcode",
        async: true,
        data: form.serialize(),
        success: (data) => {
            msg.text(data.msg);
            msg.removeClass("hidden");
        },
        error: (err) => {
            console.log(err);
        }
    })
}
