let token;
let username;
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => token })
    .build();

// аутентификация
document.getElementById("loginBtn").addEventListener("click", async () => {

    // отправляем запрос на аутентификацию
    // посылаем запрос на адрес "/login", в ответ получим токен и имя пользователя
    const response = await fetch("/auth", {
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

// отправка сообщения от простого пользователя
document.getElementById("sendBtn").addEventListener("click", () => {
    const message = document.getElementById("message").value;
    const username = document.getElementById("sendTo").value;
    hubConnection.invoke("Send", message, username)
        .catch(error => console.error(error));
});
// получение сообщения от сервера
hubConnection.on("Receive", (user, message) => {

    // создаем элемент <b> для имени пользователя
    const userNameElem = document.createElement("b");
    userNameElem.textContent = `${user}: `;

    // создает элемент <p> для сообщения пользователя
    const elem = document.createElement("p");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));

    var firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});