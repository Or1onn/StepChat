const fileInput = document.getElementById("file-input");

fileInput.addEventListener("change", function () {
    paper_clip_popup();
});


$("#fileForm").submit(async function (event) {
    event.preventDefault();
    let form = $('#fileForm')[0];
    let formData = new FormData(form);
    if (fileInput.files[0].name.indexOf(".png") !== -1 || fileInput.files[0].name.indexOf(".jpg") !== -1 || fileInput.files[0].name.indexOf(".jpeg") !== -1) {
        await createInputYourImage(fileInput.files[0], fileInput.files[0].name);
    }
    else {
        await  createInputYourFile(window.URL.createObjectURL(fileInput.files[0]), fileInput.files[0].name)
    }
    $.ajax({
        url: "/uploadFile",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (fileId) {
            hubConnection.invoke("SendFiles", fileId, userId)
                .catch(error => console.error(error));
        },
        error: function (xhr, status, error) {

        }
    });
});