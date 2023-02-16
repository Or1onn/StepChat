document.getElementById("eye").addEventListener("click", async () => {
    var pass = document.getElementById("pass");
    var eye = document.getElementById("eye");
    if (pass.type == "password") {
        eye.src = "hide_pass.svg"
        pass.type = "text"
    }
    else {
        eye.src = "show_pass.svg"
        pass.type = "password"
    }
});
