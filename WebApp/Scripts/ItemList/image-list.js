var modal = document.getElementById("image-modal");
var display = document.getElementById("display");

let close = document.getElementById("close");
close.onclick = () => { modal.style.display = "none" };

function displayImage(id) {
    display.src = "/image/get/" + id;
    modal.style.display = "block";
}