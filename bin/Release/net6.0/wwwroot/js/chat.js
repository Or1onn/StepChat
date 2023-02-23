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


async function openChat(_this) {
    if (startBoxStyle.display != "none") {
        startBoxStyle.display = "none";
        document.getElementById("chat-box").style.display = "flex";
    }

    if (_this.querySelector(".new-message_container").style.display == "flex") {
        _this.querySelector(".new-message_container").style.display = "none";
        _this.querySelector(".new-message").textContent = "";
    }

    document.getElementById("chat-name").textContent = _this.getElementsByTagName('h4')[0].textContent;
    document.getElementById("chat-image").src = _this.getElementsByTagName('img')[0].src;
    document.getElementById("messageList").innerHTML = "";

    userId = _this.getAttribute("data-email").toString();
    chatId = _this.getAttribute("data-chatId").toString();
}


$(document).on("click", ".block", async function () {
    await openChat(this);

    $.post('/getPrivateKey', { chatId: chatId }, function (response) {
        if (response != undefined) {
            privateKey = response;
            hubConnection.invoke("LoadMessages", Number(chatId))
                .catch(error => console.error(error));
        }
    });
});


$(".new-chat-user").click(async function () {
    userId = this.getAttribute("data-email").toString();

    const elements = document.getElementsByClassName('block');

    for (let i = 0; i < elements.length; i++) {
        if (userId == elements[i].getAttribute("data-email")) {
            await openChat(elements[i]);
            return;
        }
    }

    privateKey = CryptoJS.lib.WordArray.random(32).toString();
    new_chat_close();
    startBoxStyle.display = "none";
    document.getElementById("messageList").innerHTML = "";
    document.getElementById("chat-box").style.display = "flex";
    document.getElementById('message').disabled = true;

    hubConnection.invoke("StartMessaging", userId, privateKey, Number(id))
        .catch(error => console.error(error));
});

document.getElementById("message").addEventListener('keyup', async function (event) {
    if (event.keyCode === 13) {
        await sendMessage();
    }
});

async function sendMessage() {
    const message = document.getElementById("message").value;
    if (message != "") {
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
}



document.getElementById("sendBtn").addEventListener("click", async () => {
    if (document.getElementById("message").value != "") {
        await sendMessage();
    }
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


    allChatsId.push(_chatId.toString());

    document.getElementById('message').disabled = false;
    document.getElementById('chat-image').src = "data:image/png;base64," + image;
    document.getElementById('chat-name').textContent = fullName;
});

hubConnection.on("ReceiveMessage", (messages, sendId, checkChatId, chatName, image, email) => {
    const date = new Date();

    if (allChatsId.length == 0) {
        const elements = document.getElementsByClassName('block');

        for (let i = 0; i < elements.length; i++) {
            allChatsId.push(elements[i].getAttribute("data-chatId"));
        }
    }
    if (allChatsId.includes(checkChatId.toString())) {
        if (chatId == checkChatId) {
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
            $.post('/getPrivateKey', { chatId: checkChatId }, function (response) {
                if (response != undefined) {
                    privateKey = response;

                    const divElement = document.querySelector(`div[data-chatId="${checkChatId}"]`);

                    divElement.querySelector(".new-message_container").style.display = "flex";

                    if (divElement.querySelector("#message-preview") && divElement.querySelector("#message-preview").textContent != null) {
                        divElement.querySelector("#message-preview").textContent = DecryptMessage(messages, privateKey);
                    }
                    else {
                        divElement.querySelector("#message-preview").textContent = "";
                        divElement.querySelector("#message-preview").textContent = DecryptMessage(messages, privateKey);
                    }

                    const messageCount = divElement.querySelector(".new-message").textContent;

                    if (messageCount == "") {
                        divElement.querySelector(".new-message").textContent = "1";
                    }
                    else {
                        divElement.querySelector(".new-message").textContent = (Number(divElement.querySelector(".new-message").textContent) + 1).toString();
                    }
                }
            });
        }
    }
    else {
        $.post('/getPrivateKey', { chatId: checkChatId }, function (response) {
            if (response != undefined) {
                privateKey = response;

                document.getElementById('chat-list').insertAdjacentHTML(
                    'beforebegin',
                    `<div id="user-cl" class="block" data-email="${email}" data-chatId="${checkChatId}">
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
                                        <p id="message-preview">${DecryptMessage(messages, privateKey)}</p>
                                    </div>
                                    <div class="new-message_container">
                                        <div id="new_message" class="new-message">1</div>
                                    </div>
                                </div>
                            </div>
                        </div>`)

                const divElement = document.querySelector(`div[data-chatId="${checkChatId}"]`);
                divElement.querySelector(".new-message_container").style.display = "flex";

            }
        });
    }
});

hubConnection.on("ReceiveFile", (fileId) => {
    // Создать объект XMLHttpRequest
    var xhr = new XMLHttpRequest();
    var data = new FormData();

    xhr.open('POST', '/donloadFile', true);
    xhr.responseType = 'blob';
    data.append('fileId', fileId);

    xhr.onload = function () {
        let url = window.URL.createObjectURL(xhr.response);
        let filename;
        let a = document.createElement('a');

        a.href = url;

        const disposition = xhr.getResponseHeader('Content-Disposition');
        if (disposition && disposition.indexOf('attachment') !== -1) {
            let filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
            let matches = filenameRegex.exec(disposition);
            if (matches != null && matches[1]) {
                filename = matches[1].replace(/['"]/g, '');
            }
        }

        a.download = filename;
        document.body.appendChild(a);
        a.click();

        document.body.removeChild(a);
    };

    xhr.send(data);
});