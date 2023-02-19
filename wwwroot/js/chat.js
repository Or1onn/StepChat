"use strict";

let token;
let userId;
let chatId;
let privateKey;
let id;
let startBoxStyle = document.getElementById("start-box").style;
let allChatsId = [];
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


$(document).on("click", ".block", function () {
    if (startBoxStyle.display != "none") {
        startBoxStyle.display = "none";
        document.getElementById("chat-box").style.display = "flex";
    }

    document.getElementById("chat-name").textContent = this.getElementsByTagName('h4')[0].textContent;
    document.getElementById("chat-image").src = this.getElementsByTagName('img')[0].src;
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
    new_chat_close();
    startBoxStyle.display = "none";
    document.getElementById("messageList").innerHTML = "";
    document.getElementById("chat-box").style.display = "flex";
    document.getElementById('message').disabled = true;

    hubConnection.invoke("StartMessaging", userId, privateKey, Number(id))
        .catch(error => console.error(error));
});

async function sendMessage() {
    const message = document.getElementById("message").value;
    let date = new Date();
    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        ` <div class="chat-message-container-your">
                            <div class="chat-message-your">
                                <div class="chatting-your-message-text">
                                    ${message}
                                </div>
                                <span class="chat-your-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                            <div class="message-kr">
                                <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class=""
                                     version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                                <path fill="#00a884" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                                </svg>
                            </div>
                        </div>`)
    document.getElementById("message").value = "";
    let text = EncryptMessage(message);

    hubConnection.invoke("SendMessage", text, userId, Number(id))
        .catch(error => console.error(error));
}


document.getElementById("message").addEventListener('keyup', async function (event) {
    if (event.keyCode === 13) {
        await sendMessage();
    }
});

document.getElementById("sendBtn").addEventListener("click", async () => {
    await sendMessage();
});


hubConnection.on("CreateChat", (email, _chatId, fullName, image) => {
    userId = email;
    chatId = _chatId;

    const date = new Date();
    document.getElementById('chat-list').insertAdjacentHTML(
        'afterbegin',
        `<div id="user-cl" class="block" data-email=${email} data-chatId=${chatId}>
                            <div class="image-box">
                                <div class="image-padding">
                                    <img class="image" src="data:image/png;base64,${image}">
                                </div>
                            </div>
                            <div id="user-cl" class="details">
                                <div class="head-list">
                                    <h4>${fullName}</h4>
                                    <p class="time">${date.getHours() + ':' + date.getMinutes()}</p>
                                </div>
                                <div id="user-cl" class="message-container">
                                    <div class="message">
                                        <p></p>
                                    </div>
                                </div>
                            </div>
                        </div>`
    )
    document.getElementById('message').disabled = false;
});


hubConnection.on("ReceiveMessage", (messages, sendId, chatId, chatName, image, email) => {
    const date = new Date();
    var chats = document.getElementsByClassName("block");

    if (allChatsId.length == 0) {
        const elements = document.getElementsByClassName('block');

        for (let i = 0; i < elements.length; i++) {
            allChatsId.push(elements[i].getAttribute("data-chatId"));
        }
    }
    if (allChatsId.includes(chatId.toString())) {
        if (messages.constructor === Array) {
            messages.forEach(function (element) {
                if (element.userId == id) {
                    document.getElementById('messageList').insertAdjacentHTML(
                        'afterbegin',
                        `<div class="chat-message-container-your">
                            <div class="chat-message-your">
                                <div class="chatting-your-message-text">
                                     ${DecryptMessage(element.text, privateKey)}
                                </div>
                                <span class="chat-your-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                            <div class="message-kr">
                                <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class=""
                                     version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                                <path fill="#00a884" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                            </svg>
                            </div>
                        </div>`
                    )
                }
                else {
                    document.getElementById('messageList').insertAdjacentHTML(
                        'afterbegin',
                        `<div class="chat-message-container-frnd">
                            <div class="frnd-message-kr">
                                <svg viewBox="0 1.3 8 13" height="20" width="20" preserveAspectRatio="xMaxYMax meet" class=""
                                     version="1.1" x="0px" y="0px" xml:space="preserve">
                                <path fill="#3b4a54" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path>
                            </svg>
                            </div>
                            <div class="chat-message-frnd">
                                <div class="chatting-frnd-message-text">
                                     ${DecryptMessage(element.text, privateKey)}
                                </div>
                                <span class="chat-frnd-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                        </div>`
                    )
                }
            });
        }
        else {
            if (sendId == id) {
                document.getElementById('messageList').insertAdjacentHTML(
                    'afterbegin',
                    `<div class="chat-message-container-your">
                            <div class="chat-message-your">
                                <div class="chatting-your-message-text">
                                     ${DecryptMessage(messages, privateKey)}
                                </div>
                                <span class="chat-your-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                            <div class="message-kr">
                                <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class=""
                                     version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                                <path fill="#00a884" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                            </svg>
                            </div>
                        </div>`
                )
            }
            else {
                document.getElementById('messageList').insertAdjacentHTML(
                    'afterbegin',
                    `<div class="chat-message-container-frnd">
                            <div class="frnd-message-kr">
                                <svg viewBox="0 1.3 8 13" height="20" width="20" preserveAspectRatio="xMaxYMax meet" class=""
                                     version="1.1" x="0px" y="0px" xml:space="preserve">
                                <path fill="#3b4a54" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path>
                                </svg>
                            </div>
                            <div class="chat-message-frnd">
                                <div class="chatting-frnd-message-text">
                                    ${DecryptMessage(messages, privateKey)}
                                </div>
                                <span class="chat-frnd-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                        </div>`
                )
            }
        }
    }
    else {
        $.post('/getPrivateKey', { chatId: chatId }, function (response) {
            if (response != undefined) {
                privateKey = response;

                document.getElementById('chat-list').insertAdjacentHTML(
                    'beforebegin',
                    `<div id="user-cl" class="block" data-email="${email}" data-chatId="${chatId}">
                            <div class="image-box">
                                <div class="image-padding">
                                    <img class="image" src="data:image;base64,${image}">
                                </div>
                            </div>
                            <div id="user-cl" class="details">
                                <div class="head-list">
                                    <h4>${chatName}</h4>
                                    <p class="time">${date.getHours() + ':' + date.getMinutes()}</p>
                                </div>
                                <div id="user-cl" class="message-container">
                                    <div class="message">
                                        <p>${DecryptMessage(messages, privateKey)}</p>
                                    </div>
                                </div>
                            </div>
                        </div>`)
            }
        });
    }
});