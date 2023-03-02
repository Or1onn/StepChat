async function createYourMessage(message, privateKey) {
    let date = new Date();
    let decryptedMessage = DecryptMessage(message, privateKey);

    if (decryptedMessage !== "") {
        message = decryptedMessage;
    }

    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        `<div class="chat-message-container-your">
                                <div class="chat-message-your">
                                    <div class="chatting-your-message-text">${message}</div>
                                    <span class="chat-your-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                                </div>
                                <div class="message-kr">
                                    <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class=""
                                         version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                                    <path fill="#00a199" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                                </svg>
                                </div>
                            </div>`
    )
}


async function createFriendMessage(message, privateKey) {
    let date = new Date();

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
                                     ${DecryptMessage(message, privateKey)}
                                </div>
                                <span class="chat-frnd-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                        </div>`
    )
}


async function createInputYourMessage(message, privateKey) {
    let date = new Date();
    let decryptedMessage = DecryptMessage(message, privateKey);

    if (decryptedMessage !== "") {
        message = decryptedMessage;
    }

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-your">
                                <div class="chat-message-your">
                                    <div class="chatting-your-message-text">${message}</div>
                                    <span class="chat-your-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                                </div>
                                <div class="message-kr">
                                    <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class=""
                                         version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                                    <path fill="#00a199" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                                </svg>
                                </div>
                            </div>`
    )
}


async function createInputFriendMessage(message, privateKey) {
    let date = new Date();

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-frnd">
                            <div class="frnd-message-kr">
                                <svg viewBox="0 1.3 8 13" height="20" width="20" preserveAspectRatio="xMaxYMax meet" class=""
                                     version="1.1" x="0px" y="0px" xml:space="preserve">
                                <path fill="#3b4a54" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path>
                            </svg>
                            </div>
                            <div class="chat-message-frnd">
                                <div class="chatting-frnd-message-text">
                                     ${DecryptMessage(message, privateKey)}
                                </div>
                                <span class="chat-frnd-message-time">${date.getHours() + ':' + date.getMinutes()}</span>
                            </div>
                        </div>`
    )
}


