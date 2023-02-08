"use strict";

let token;
let userId;
let privateKey;

function EncryptMessage(message, key) {
    var iv = CryptoJS.lib.WordArray.random(25).toString();
    //var key2 = CryptoJS.lib.WordArray.random(32).toString();

    var encrypted = CryptoJS.AES.encrypt(message, privateKey, { iv: iv }).toString();

    var context = {
        Text: encrypted,
        PrivateKey: privateKey,
        IV: iv
    };

    return context;
}

function DecryptMessage(context) {
    return CryptoJS.AES.decrypt(context.text, context.privateKey, { iv: context.iv }).toString(CryptoJS.enc.Utf8);
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
    privateKey = CryptoJS.lib.WordArray.random(32).toString();

    hubConnection.invoke("StartMessaging", userId, privateKey)
        .catch(error => console.error(error));
});

const divs = document.querySelectorAll("div");
divs.forEach(function (div) {
    div.addEventListener("click", function () {
        if (this.getAttribute("data-email") != null) {
            userId = this.getAttribute("data-email").toString();
        }
    });
});

document.getElementById("sendBtn").addEventListener("click", () => {
    const message = document.getElementById("message").value;

    let context = EncryptMessage(message);

    hubConnection.invoke("SendMessage", context, userId)
        .catch(error => console.error(error));
});

// получение сообщения от сервера
hubConnection.on("ReceiveMessage", (context) => {
    let message = DecryptMessage(context);

    document.getElementById('messageBox').insertAdjacentHTML(
        'afterbegin',
        `<p>${message}?<br><span>12:15</span></p>`
    )

}

    //// создаем элемент <b> для имени пользователя
    //const userIdElem = document.createElement("b");
    //userIdElem.textContent = `User: `;

    //// создает элемент <p> для сообщения пользователя
    //const elem = document.createElement("p");
    //elem.appendChild(userIdElem);
    //elem.appendChild(document.createTextNode(message));

    //var firstElem = document.getElementById("chatroom").firstChild;
    //document.getElementById("chatting").insertBefore(elem, firstElem);
);



hubConnection.on("SendPrivateKeys", (key) => {
    let message = DecryptMessage(context);
});

