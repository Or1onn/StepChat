"use strict";

let token;
let userId;
let chatId;
let privateKey;
let id;
let startBoxStyle = document.getElementById("start-box").style;

function EncryptMessage(message) {
    var encrypted = CryptoJS.AES.encrypt(message, privateKey).toString();

    return encrypted;
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
        hubConnection.start()
            .catch(err => console.error(err.toString()));
    }).catch(function (error) {
        console.log('request failed', error)
    });

id = document.getElementById("userId").value;


$(".block").click(function () {
    if (startBoxStyle.display != "none") {
        startBoxStyle.display = "none";
        document.getElementById("chat-box").style.display = "flex";
    }
    document.getElementById("messageList").innerHTML = "";
    userId = this.getAttribute("data-email").toString();
    chatId = this.getAttribute("data-chatId").toString();
    $.post('/getPrivateKey', { chatId: chatId }, async function (response) {
        if (response != undefined) {
            privateKey = response;
            hubConnection.invoke("LoadMessages", privateKey, Number(chatId))
                .catch(error => console.error(error));
        }
    });
});


$(".new-chat-user").click(function () {
    userId = this.getAttribute("data-email").toString();
    privateKey = CryptoJS.lib.WordArray.random(32).toString();


    hubConnection.invoke("StartMessaging", userId, privateKey, Number(id))
        .catch(error => console.error(error));
});

document.getElementById("sendBtn").addEventListener("click", () => {
    const message = document.getElementById("message").value;

    let text = EncryptMessage(message);

    hubConnection.invoke("SendMessage", text, userId, Number(id))
        .catch(error => console.error(error));
});

hubConnection.on("ReceiveMessage", (messages, privateKey) => {
    messages.forEach(function (element) {
        let date = new Date(element.createTime);
        document.getElementById('messageList').insertAdjacentHTML(
            'afterbegin',
            `<div class="chatting-frnd-message">
                            <p>
                                ${DecryptMessage(element.text, privateKey)}
                                <span>${date.getHours() + ':' + date.getMinutes()}</span>
                            </p>
                        </div>`
        )
    });
});

