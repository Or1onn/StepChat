const fileInput = document.getElementById("file-input");

fileInput.addEventListener("change", function () {
    file = fileInput.files[0];
    document.getElementById("file-upload").style.display = "block";
});


$("#fileForm").submit(function (event) {
    
    event.preventDefault();
    var form = $('#myForm')[0];
    var formData = new FormData(form);

    $.ajax({
        url: "/uploadFile",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
           
        },
        error: function (xhr, status, error) {
            
        }
    });
});