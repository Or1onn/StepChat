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

document.getElementById("phoneInput").addEventListener("input", function () {
    var regex = /^\994(?:50|51|55|70|77)\d{7}$|^\+994\d{2}\d{7}$/gm;
    if (!regex.test(this.value)) {
        document.getElementById("phone-container").style.border = "3px solid red";
    }
    else {
        document.getElementById("phone-container").style.border = "none";
    }
});