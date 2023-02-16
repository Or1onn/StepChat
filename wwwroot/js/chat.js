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
    let date = new Date();
    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        `<div class="chat-message-container-your">
                            <div class="chat-message-your">
                                <div class="chatting-your-message-text">
                                    ${message}
                                </div>
                                <span class="chat-your-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                                <div class="message-kr">
                                    <svg viewBox="0 0.65 8 13" height="30" width="30" preserveAspectRatio="xMidYMid meet" class="" version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                                    <path fill="#00a884" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                  </svg>
                                </div>
                            </div>
                        </div>`)

    let text = EncryptMessage(message);

    hubConnection.invoke("SendMessage", text, userId, Number(id))
        .catch(error => console.error(error));
});

hubConnection.on("ReceiveMessage", (messages, privateKey) => {
    let date = new Date(element.createTime);
    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        `<div class="chat-message-container-frnd">
                            <div class="chat-message-frnd">
                                <div class="frnd-message-kr">
                                    <svg viewBox="0 0.3 8 13" height="25" width="25" preserveAspectRatio="xMidYMid meet" class="" version="1.1" x="0px" y="0px" xml:space="preserve">
                                    <path fill="#3b4a54" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path>
                  </svg>
                                </div>
                                <div class="chatting-frnd-message-text">
                                    ${DecryptMessage(message, privateKey)}
                                </div>
                                <span class="chat-frnd-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                        </div>`

    )
});

