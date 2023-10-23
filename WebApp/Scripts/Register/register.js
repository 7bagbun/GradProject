$(document).ready(() => {
    $("#btn-upload-pfp").on("click", showFilePicker);
    $("#btn-submit").on("click", validateForm);
})
function showFilePicker(e) {
    e.preventDefault();
    $("#pfp").click();
}

function showPickedImage(input) {
    const reader = new FileReader();
    reader.onload = e => {
        let fileUrl = e.target.result;
        $("#pfp-display").attr("src", fileUrl);
    };

    reader.readAsDataURL(input.files[0]);
}

function validateForm(e) {
    const username = $("#Username").val();
    const email = $("#Email").val();
    const password = $("#Password").val();
    const confirm = $("#confirm").val();

    if (username === "" || email === ""
        || password === "" || confirm === "") {
        setErrMsg("請確實輸入所有必填欄位。");
        return false;
    } else if (password !== confirm) {
        setErrMsg("密碼和確認密碼不相同");
        return false;
    }

    msg.text("");
    return true;
}

function setErrMsg(msg) {
    $("#msg").text(msg);
}