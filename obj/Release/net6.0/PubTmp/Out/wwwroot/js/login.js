document.getElementById("eye").addEventListener("click", async () => {
    var pass = document.getElementById("password");
    var eye = document.getElementById("eye");
    if (pass.type == "password") {
        eye.src = "/icons/hide_pass.svg"
        pass.type = "text"
    }
    else {
        eye.src = "/icons/show_pass.svg"
        pass.type = "password"
    }
});