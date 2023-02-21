function admin_panel() {
    document.getElementById("admin-popup").style.display = "block";
    document.getElementById("search-box").style.filter = "blur(2px)";
    document.getElementById("chat-box").style.filter = "blur(2px)";
}

function admin_close() {
    document.getElementById("search-box").style.filter = "blur(0px)";
    document.getElementById("chat-box").style.filter = "blur(0px)";
    document.getElementById("admin-popup").style.display = "none";
}

function new_chat() {
    document.getElementById("new-chat-popup").style.top = "5%";
    document.getElementById("new-chat-popup").style.display = "inline-table";
}

function new_chat_close() {
    document.getElementById("new-chat-popup").style.display = "none";
}

function user_add() {
    document.getElementById("user-popup").style.display = "block";
}

function user_close() {
    document.getElementById("user-popup").style.display = "none";
}

function user_delete() {
    document.getElementById("user-popup").style.display = "block";
}

function search_input() {
    document.getElementById("convo-search-input").style.display = "flex";
    document.getElementById("convo-search").style.display = "none";
}

function search_close() {
    document.getElementById("convo-search-input").style.display = "none";
    document.getElementById("convo-search").style.display = "flex";
}

function group_create() {
    document.getElementById("select-user").style.display = "flex";
    document.getElementById("group-create-submit").style.display = "flex";
    document.getElementById("group-buttons").style.display = "none";
}