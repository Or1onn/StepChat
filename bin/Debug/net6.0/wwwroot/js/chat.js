"use strict";

let token;
let userId;
let privateKey;
let response;

function EncryptMessage(message) {
    var encrypted = CryptoJS.AES.encrypt(message, privateKey).toString();

    var context = {
        Text: encrypted,
        PrivateKey: privateKey
    };

    return context;
}

function DecryptMessage(text, privateKey) {
    return CryptoJS.AES.decrypt(text, privateKey).toString(CryptoJS.enc.Utf8);
}


const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => token })
    .build();


fetch("/getToken")
    .then(data => token = data.text())
    .then(() => {
        hubConnection.start()       // начинаем соединение с хабом
            .catch(err => console.error(err.toString()));
    }).catch(function (error) {
        console.log('request failed', error)
    });





//hubConnection.invoke("StartMessaging", userId, privateKey)
//    .catch(error => console.error(error));


$(".block").click(function () {
    userId = this.getAttribute("data-email").toString();
    $.post('/getPrivateKey', { email: userId }, async function (data) {
        response = data.value;
        if (response != undefined) {
            hubConnection.invoke("LoadMessages", response)
                .catch(error => console.error(error));
        }

    });
});

document.getElementById("sendBtn").addEventListener("click", () => {
    const message = document.getElementById("message").value;

    let text = EncryptMessage(message);

    hubConnection.invoke("SendMessage", text, userId)
        .catch(error => console.error(error));
});

hubConnection.on("ReceiveMessage", (text, privateKey) => {
    let message = DecryptMessage(text, privateKey);

    document.getElementById('messageBox').insertAdjacentHTML(
        'afterbegin',
        `<p>${message}?<br><span>12:15</span></p>`
    )
});

