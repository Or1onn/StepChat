async function createYourMessage(message, privateKey, createTime) {
    let date = new Date(createTime);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;
    
    let decryptedMessage = DecryptMessage(message, privateKey);

    if (decryptedMessage !== "") {
        message = decryptedMessage;
    }

    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        `<div class="chat-message-container-your">
                                <div class="chat-message-your">
                                    <div class="chatting-your-message-text">${message}</div>
                                    <span class="chat-your-message-time">${formattedTime}</span>
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


async function createFriendMessage(message, privateKey, createTime) {
    let date = new Date(createTime);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;
    
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
                                <span class="chat-frnd-message-time">${formattedTime}</span>
                            </div>
                        </div>`
    )
}

async function createYourImage(image, fileName) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;

    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        `<div class="chat-message-your-img">
                <div class="image-message">
                  <img src="${image}">
                </div>
                <div class="image-message-name">
                  <p>${fileName}</p>
                </div>
                <div class="your-message-img-status">
                  <span class="chat-your-img-time">${formattedTime}</span>
                </div>
              </div>`
    )
}


async function createFriendImage(image, fileName) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;
    
    document.getElementById('messageList').insertAdjacentHTML(
        'afterbegin',
        `<div class="chat-message-frnd-img">
                <div class="image-frnd-message">
                  <img src="${image}">
                </div>
                <div class="image-message-name">
                  <p>${fileName}</p>
                </div>
                <span class="chat-frnd-img-time">${formattedTime}</span>
              </div>`
    )
}

async function createInputYourMessage(message, privateKey) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;
    
    let decryptedMessage = DecryptMessage(message, privateKey);

    if (decryptedMessage !== "") {
        message = decryptedMessage;
    }

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-your">
                                <div class="chat-message-your">
                                    <div class="chatting-your-message-text">${message}</div>
                                    <span class="chat-your-message-time">${formattedTime}</span>
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
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;
    
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
                                <span class="chat-frnd-message-time">${formattedTime}</span>
                            </div>
                        </div>`
    )
}


async function createInputYourImage(image, fileName) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-your">
              <div class="chat-message-your-img">
                <div class="image-message">
                  <img class="image-preview" src="${URL.createObjectURL(image)}">
                </div>
                <div class="your-message-img-status">
                  <span class="chat-your-img-time">${formattedTime}</span>
                </div>
              </div>
              <div class="message-kr">
                <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class="" version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                  <path fill="#00a199" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                </svg>
              </div>
            </div>`
    )
}

async function createInputFriendImage(image, fileName) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;
    const blob = new Blob([image], { type: "image/png" });

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-frnd">
              <div class="frnd-message-kr">
                <svg viewBox="0 1.3 8 13" height="20" width="20" preserveAspectRatio="xMaxYMax meet" class="" version="1.1" x="0px" y="0px" xml:space="preserve">
                  <path fill="#3b4a54" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path>
                </svg>
              </div>
              <div class="chat-message-frnd-img">
                <div class="image-frnd-message">
                  <img class="image-preview" src="${URL.createObjectURL(blob)}">
                </div>
                <span class="chat-frnd-img-time">${formattedTime}</span>
              </div>
            </div>`
    )
}

async function createInputYourFile(url, filename) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-your">
              <div class="chat-message-your-file">
                <div class="your-message-file">
                  <p style="margin-left: auto; margin-right: auto;">${filename}</p>
                  <a href="${url}" class="your-message-file-btn" type="button" download="${filename}">Download</a>
                </div>
                <div class="your-message-file-status">
                  <span class="chat-your-file-time">${formattedTime}</span>
                </div>
              </div>
              <div class="message-kr">
                <svg viewBox="1 1 8 13" preserveAspectRatio="xMinYMin meet" height="20" width="20" class="" version="1.1" x="0px" y="0px" enable-background="new 0 0 8 13" xml:space="preserve">
                  <path fill="#00a199" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path>
                </svg>
              </div>
            </div>`
    )
}

async function createInputFriendFile(url, filename) {
    let date = new Date();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const formattedTime = `${hours}:${minutes}`;

    document.getElementById('messageList').insertAdjacentHTML(
        'beforeend',
        `<div class="chat-message-container-frnd">
              <div class="frnd-message-kr">
                <svg viewBox="0 1.3 8 13" height="20" width="20" preserveAspectRatio="xMaxYMax meet" class="" version="1.1" x="0px" y="0px" xml:space="preserve">
                  <path fill="#3b4a54" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path>
                </svg>
              </div>
              <div class="chat-message-frnd-file">
                <div class="frnd-message-file">
                  <p style="margin-left: auto; margin-right: auto;">${filename}</p>
                  <a href="${url}" class="frnd-message-file-btn" type="button" download="${filename}">Download</a>
                </div>
                <span class="chat-frnd-file-time">${formattedTime}</span>
              </div>
            </div>`
    )
}


