$(document).ready(() => {
    $("#comment-tab").on("click", null, id, loadComments);
    $("#list-tab").on("click", null, "list", changeTabParam);
    $("#comment-tab").on("click", null, "comment", changeTabParam);

    if (tab !== null) {
        $(`#${tab}-tab`).click();
    }
});
function changeTabParam(e) {
    window.history.replaceState(null, null, "?tab=" + e.data);
}
