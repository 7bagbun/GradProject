$(document).ready(() => {
    $("#btn-upload-pfp").on("click", showFilePicker);
    $("#btn-submit").on("click", submitForm);
    $("#pfp-clear").on("click", clearPfp);
    $("#img-section").on("click", showFilePicker);
    $("#img-section").on("dragover", dropOver);
    $("#img-section").on("drop", dropImage);
})

function showFilePicker(e) {
    e.preventDefault();
    $("#pfp").click();
}

function showPickedImage() {
    const file = $("#pfp");
    const reader = new FileReader();
    reader.onload = e => {
        let fileUrl = e.target.result;
        $("#pfp-display").attr("src", fileUrl);
    };

    reader.readAsDataURL(file.prop("files")[0]);
    $("#pfp-clear").removeClass("hidden");
}

function validateForm() {
    const username = $("#Username").val();
    const email = $("#Email").val();
    const password = $("#Password").val();
    const confirm = $("#confirm").val();

    if (username === "" || email === ""
        || password === "" || confirm === "") {
        setErrMsg("請確實輸入所有必填欄位。");
        return false;
    } else if (!validateEmail(email)) {
        setErrMsg("電子信箱格式無效");
        return false;
    } else if (password !== confirm) {
        setErrMsg("密碼和確認密碼不相同");
        return false;
    }

    setErrMsg("");
    return true;
}

function submitForm(e) {
    e.preventDefault()

    if (!validateForm()) {
        return;
    }

    $.ajax({
        type: "POST",
        url: "/member/register",
        async: true,
        data: new FormData(document.getElementById("rp")),
        contentType: false,
        processData: false,
        success: (data) => {
            if (data.isSucceed) {
                window.location = data.redirUrl;
            } else {
                setErrMsg(data.msg);
            }
        },
        failure: (err) => {
            console.log(err);
        }
    })
}

function setErrMsg(msg) {
    $("#msg").text(msg);
}

function clearPfp(e) {
    e.stopPropagation();
    $("#pfp").val(null);
    $("#pfp-clear").addClass("hidden");
    $("#pfp-display").attr("src", "/Assets/Images/drag-img.png")
}

function dropOver(e) {
    e.preventDefault();
}
function dropImage(e) {
    e.preventDefault();
    $("#pfp").prop("files", e.originalEvent.dataTransfer.files);
    showPickedImage();
}

function validateEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}