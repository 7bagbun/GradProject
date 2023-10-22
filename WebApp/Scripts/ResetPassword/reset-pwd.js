$(document).ready(() => {
    $("#btn-submit").on("click", submitForm);
});

function submitForm(e) {
    e.preventDefault();
    const form = $("#rp");
    const msg = $("#msg");
    const pwd = $("#newPassword");
    const confirmPwd = $("#confirm");
    msg.css("display", "block");

    if (pwd.val() === "" || confirmPwd.val() === "") {
        msg.text("請確實輸入所有欄位。");
        msg.css("display", "block");
        return;
    } else if (pwd.val() !== confirmPwd.val()) {
        msg.text("新密碼與確認新密碼不相同。");
        msg.css("display", "block");
        return;
    }

    form.submit();
}
