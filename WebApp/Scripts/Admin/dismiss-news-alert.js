if (document.cookie.indexOf("dismissAlert=true") < 0) {
    $("#alert-link").removeClass("hidden");
}

function rememberDismiss() {
    document.cookie = "dismissAlert=true; path=/admin; sameSite=strict"
}