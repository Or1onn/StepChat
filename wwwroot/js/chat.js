function EncryptMessage(message, key) {
    var iv = CryptoJS.lib.WordArray.random(25).toString();
    var key2 = CryptoJS.lib.WordArray.random(32).toString();

    var encrypted = CryptoJS.AES.encrypt(message, key2, { iv: iv }).toString();

    var context = {
        Text: encrypted,
        PrivateKey: key2,
        IV: iv
    };

    return context;
}

function DecryptMessage(context) {
    return CryptoJS.AES.decrypt(context.text, context.privateKey, { iv: context.iv }).toString(CryptoJS.enc.Utf8);
}

let token;
let username;

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => token })
    .build();


document.getElementById("loginBtn").addEventListener("click", async () => {
    const response = await fetch("/log", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            email: document.getElementById("email").value,
        })
    });

        // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const data = await response.json();
        token = data.access_token;
        username = data.username;
        document.getElementById("loginBtn").disabled = true;

        hubConnection.start()       // начинаем соединение с хабом
            .then(() => document.getElementById("sendBtn").disabled = false)
            .catch(err => console.error(err.toString()));
    }
    else {
        // если произошла ошибка, получаем код статуса
        console.log(`Status: ${response.status}`);
    }
    
});

// аутентификация
document.getElementById("registerBtn").addEventListener("click", async () => {

    // отправляем запрос на аутентификацию
    // посылаем запрос на адрес "/auth", в ответ получим токен и имя пользователя
    const response = await fetch("/reg", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            email: document.getElementById("reg_email").value,
        })
    });

});

// отправка сообщения от простого пользователя
document.getElementById("sendBtn").addEventListener("click", () => {
    const message = document.getElementById("message").value;
    const username = document.getElementById("sendTo").value;


    let context = EncryptMessage(message);

    hubConnection.invoke("SendMessage", context, username)
        .catch(error => console.error(error));
});

// получение сообщения от сервера
hubConnection.on("ReceiveMessage", (context) => {
    let message = DecryptMessage(context);

    // создаем элемент <b> для имени пользователя
    const userNameElem = document.createElement("b");
    userNameElem.textContent = `User: `;

    // создает элемент <p> для сообщения пользователя
    const elem = document.createElement("p");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));

    var firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

hubConnection.on("SendPrivateKeys", (key) => {
    let message = DecryptMessage(context);

    
});