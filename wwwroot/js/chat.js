"use strict";

let token;
let userId;
let chatId;
let privateKey;
let id;
let startBoxStyle = document.getElementById("start-box").style;
let isScrollIgnored = false;
let messagesCount = 0;
let allChatsId = [];

function EncryptMessage(message) {
    var encrypted = CryptoJS.AES.encrypt(message, privateKey).toString();

    return encrypted;
}

function DecryptMessage(text, privateKey) {
    return CryptoJS.AES.decrypt(text, privateKey).toString(CryptoJS.enc.Utf8);
}


const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {accessTokenFactory: () => token})
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
    if (startBoxStyle.display !== "none") {
        startBoxStyle.display = "none";
        document.getElementById("chat-box").style.display = "flex";
    }

    if (_this.querySelector(".new-message_container").style.display === "flex") {
        _this.querySelector(".new-message_container").style.display = "none";
        _this.querySelector(".new-message").textContent = "";
    }

    document.getElementById("chat-name").textContent = _this.getElementsByTagName('h4')[0].textContent;
    document.getElementById("chat-image").src = _this.getElementsByTagName('img')[0].src;
    document.getElementById("messageList").innerHTML = "";

    userId = _this.getAttribute("data-email").toString();
    chatId = _this.getAttribute("data-chatId").toString();

    await checkPrivateKeys(chatId)
        .then(returnedPrivateKey => {
            privateKey = returnedPrivateKey;
            hubConnection.invoke("LoadMessages", Number(chatId), messagesCount)
                .catch(error => console.log(error));
        }).catch(error => console.log(error));
}


async function sendMessage() {
    const message = document.getElementById("message").value;
    if (message !== "") {

        await createInputYourMessage(message, privateKey);

        document.getElementById("message").value = "";

        document.getElementById("messageList").scrollTo(0, document.getElementById("messageList").scrollHeight);

        let text = EncryptMessage(message);

        hubConnection.invoke("SendMessage", text, userId, Number(id))
            .catch(error => console.error(error));
    }
}

async function checkPrivateKeys(chatId) {
    return new Promise((resolve, reject) => {
        let privateKey = localStorage.getItem(chatId);

        if (privateKey !== null) {
            resolve(privateKey);
        } else {
            $.post('/getPrivateKey', {chatId: chatId}, function (response) {
                if (response !== undefined) {
                    privateKey = response;
                    localStorage.setItem(chatId, privateKey);
                    resolve(response);
                } else {
                    reject('Error: Failed to get private key');
                }
            });
        }
    });
}

document.getElementById('messageList').addEventListener("scroll", function () {
    if (isScrollIgnored) return;

    if (this.scrollTop === 0) {
        hubConnection.invoke("LoadMessages", Number(chatId), messagesCount += 30)
            .catch(error => console.error(error));
    }
});


$(document).on("click", ".block", async function () {
    if ($(window).width() <= 800) {
        document.getElementById('search-box').style.display = "none";
    }

    isScrollIgnored = true;
    messagesCount = 0;
    await openChat(this);
});

$(".new-chat-user").click(async function () {
    userId = this.getAttribute("data-email").toString();

    const elements = document.getElementsByClassName('block');

    for (let i = 0; i < elements.length; i++) {
        if (userId === elements[i].getAttribute("data-email")) {
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

document.getElementById("sendBtn").addEventListener("click", async () => {
    if (document.getElementById("message").value !== "") {
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

    localStorage.setItem(chatId, privateKey);
    document.getElementById('message').disabled = false;
    document.getElementById('chat-image').src = "data:image/png;base64," + image;
    document.getElementById('chat-name').textContent = fullName;
});

hubConnection.on("ReceiveMessage", async (messages, sendId, checkChatId, chatName, image, email) => {
    const date = new Date();

    if (allChatsId.length === 0) {
        const elements = document.getElementsByClassName('block');

        for (let i = 0; i < elements.length; i++) {
            allChatsId.push(elements[i].getAttribute("data-chatId"));
        }
    }
    if (!document.hidden) {
        if (allChatsId.includes(checkChatId.toString())) {
            if (chatId === checkChatId.toString()) {
                if (messages.constructor !== Array) {
                    if (sendId === id) {
                        await createYourMessage(messages, privateKey);
                    } else {
                        await createInputFriendMessage(messages, privateKey)
                    }
                } else {
                    for (const element of messages) {
                        if (element.userId === Number(id)) {
                            await createYourMessage(element.text, privateKey);
                        } else {
                            await createFriendMessage(element.text, privateKey)
                        }
                    }

                    document.getElementById("messageList").scrollTo(0, 100);

                    if (messagesCount === 0) {
                        document.getElementById("messageList").scrollTo(0, document.getElementById("messageList").scrollHeight);
                    }
                    isScrollIgnored = false;
                    return;
                }
            } else {
                if (messages.constructor !== Array) {
                    await checkPrivateKeys(chatId)
                        .then(returnedPrivateKey => {
                            privateKey = returnedPrivateKey;
                            const divElement = document.querySelector(`div[data-chatId="${checkChatId}"]`);

                            divElement.querySelector(".new-message_container").style.display = "flex";

                            if (divElement.querySelector("#message-preview") && divElement.querySelector("#message-preview").textContent != null) {
                                divElement.querySelector("#message-preview").textContent = DecryptMessage(messages, privateKey);
                            } else {
                                divElement.querySelector("#message-preview").textContent = "";
                                divElement.querySelector("#message-preview").textContent = DecryptMessage(messages, privateKey);
                            }

                            const messageCount = divElement.querySelector(".new-message").textContent;

                            if (messageCount === "") {
                                divElement.querySelector(".new-message").textContent = "1";
                            } else {
                                divElement.querySelector(".new-message").textContent = (Number(divElement.querySelector(".new-message").textContent) + 1).toString();
                            }
                            document.getElementById("new-message").play();
                        }).catch(error => console.log(error));
                }
            }
        } else {
            await checkPrivateKeys(chatId)
                .then(returnedPrivateKey => {
                    privateKey = returnedPrivateKey;

                    document.getElementById('chat-list').insertAdjacentHTML(
                        'beforebegin',
                        `<div id="user-cl" class="block" data-email="${email}" data-chatId="${checkChatId}">
                            <div class="image-box">
                                <div class="image-padding">
                                    <img class="image" src="data:image;base64,${image}" alt="avatar">
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

                    if (!allChatsId.includes(checkChatId.toString())) {
                        allChatsId.push(checkChatId.toString());
                    }
                }).catch(error => console.log(error));
        }
    } else {
        await checkPrivateKeys(chatId)
            .then(async returnedPrivateKey => {
                privateKey = returnedPrivateKey;

                const divElement = document.querySelector(`div[data-chatId="${checkChatId}"]`);

                divElement.querySelector(".new-message_container").style.display = "flex";

                if (divElement.querySelector("#message-preview") && divElement.querySelector("#message-preview").textContent != null) {
                    divElement.querySelector("#message-preview").textContent = DecryptMessage(messages, privateKey);
                } else {
                    divElement.querySelector("#message-preview").textContent = "";
                    divElement.querySelector("#message-preview").textContent = DecryptMessage(messages, privateKey);
                }

                const messageCount = divElement.querySelector(".new-message").textContent;

                if (messageCount === "") {
                    divElement.querySelector(".new-message").textContent = "1";
                } else {
                    divElement.querySelector(".new-message").textContent = (Number(divElement.querySelector(".new-message").textContent) + 1).toString();
                }

                if (chatId === checkChatId.toString()) {
                    await createInputFriendMessage(messages, privateKey)
                }

                document.getElementById("new-message").play();
            }).catch(error => console.log(error));
    }
});

hubConnection.on("ReceiveFile", (fileId) => {
    let xhr = new XMLHttpRequest();
    let data = new FormData();

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