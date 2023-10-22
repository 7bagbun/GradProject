$(document).ready(() => {
    $("#btn-submit").on("click", submitForm);
});

function submitForm(e) {
    e.preventDefault();
    const form = $("#fp");
    const msg = $("#msg");
    const input = $("#email");
    msg.css("display", "block");

    if (input.val() === "") {
        msg.text("請輸入電子信箱。");
        msg.css("display", "block");
        return;
    }

    $.ajax({
        type: "POST",
        url: "/forgetpassword/sendemailcode",
        async: true,
        data: form.serialize(),
        success: (data) => {
            msg.text(data.msg);
            msg.css("display", "block");
        },
        failure: (err) => {
            console.log(err);
        }
    })
}
