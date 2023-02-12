"use strict";

let token;
let userId;
let privateKey;

function EncryptMessage(message) {
    var encrypted = CryptoJS.AES.encrypt(message, privateKey).toString();

    var context = {
        Text: encrypted,
        PrivateKey: privateKey
    };

    return context;
}

function DecryptMessage(context) {
    return CryptoJS.AES.decrypt(context.text, context.privateKey).toString(CryptoJS.enc.Utf8);
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



document.getElementById("messageStart").addEventListener("click", () => {

    hubConnection.invoke("LoadMessages", userId)
        .catch(error => console.error(error));

    //hubConnection.invoke("StartMessaging", userId, privateKey)
    //    .catch(error => console.error(error));
});


$(document).ready(function () {
    $(".block").click(function () {
        userId = this.getAttribute("data-email").toString();
        $.post('/getPrivateKey', { email: userId }, function (data) {
            privateKey = data
        });
    });
});

// const divs = document.querySelectorAll("div");
// divs.forEach(function (div) {
//     div.addEventListener("click", function () {
//         if (this.getAttribute("data-email") != null) {
//             userId = this.getAttribute("data-email").toString();
//             $.post('/getPrivateKey', { email: userId }, function (data) {
//                 privateKey = data
//             });
//         }
//     })
// });

document.getElementById("sendBtn").addEventListener("click", () => {
    const message = document.getElementById("message").value;

    let text = EncryptMessage(message);

    hubConnection.invoke("SendMessage", text, userId)
        .catch(error => console.error(error));
});

hubConnection.on("ReceiveMessage1", (context) => {
    let message = DecryptMessage(context);

    document.getElementById('messageBox').insertAdjacentHTML(
        'afterbegin',
        `<p>${message}?<br><span>12:15</span></p>`
    )

});

// получение сообщения от сервера
hubConnection.on("ReceiveMessage", (text) => {
    let message = DecryptMessage(text);

    document.getElementById('messageBox').insertAdjacentHTML(
        'afterbegin',
        `<p>${message}?<br><span>12:15</span></p>`
    )

});


hubConnection.on("SendPrivateKeys", (key) => {
    let message = DecryptMessage(context);
});

