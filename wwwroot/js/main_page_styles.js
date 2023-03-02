function admin_panel() {
    document.getElementById("admin-popup").style.display = "block";
}

function admin_close() {
    document.getElementById("admin-popup").style.display = "none";
}

function new_chat() {
    document.getElementById("new-chat-popup").style.top = "5%";
    document.getElementById("new-chat-popup").style.display = "inline-table";
}

function new_chat_close() {
    document.getElementById("new-chat-popup").style.display = "none";
}

function emoji_popup() {
    document.getElementById("emoji-popup").style.display = "flex";
    document.getElementById("emoji-close-btn").style.display = "flex";
}

function emoji_close() {
    document.getElementById("emoji-popup").style.display = "none";
    document.getElementById("emoji-close-btn").style.display = "none";
}

function paper_clip_popup() {
    document.getElementById("paper-clip-popup").style.display = "flex";
}

function paper_clip_close() {
    document.getElementById("paper-clip-popup").style.display = "none";
}

function user_close() {
    document.getElementById("user-popup").style.display = "none";
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
    document.getElementById("admin-popup").style.height = "60%";
    document.getElementById("admin-popup").style.top = "10%";
    document.getElementById("select-user").style.display = "flex";
    document.getElementById("group-create-submit").style.display = "none";
    document.getElementById("group-edit-submit").style.display = "flex";
    document.getElementById("user-delete-submit").style.display = "none";
    document.getElementById("user-add-submit").style.display = "none";
    document.getElementById("group-buttons").style.display = "none";
}

function user_delete() {
    document.getElementById("admin-popup").style.height = "60%";
    document.getElementById("admin-popup").style.top = "10%";
    document.getElementById("select-user").style.display = "flex";
    document.getElementById("group-create-submit").style.display = "none";
    document.getElementById("group-edit-submit").style.display = "none";
    document.getElementById("user-add-submit").style.display = "none";
    document.getElementById("user-delete-submit").style.display = "flex";
    document.getElementById("group-buttons").style.display = "none";
}

function user_add() {
    document.getElementById("admin-popup").style.height = "60%";
    document.getElementById("admin-popup").style.top = "10%";
    document.getElementById("select-user").style.display = "flex";
    document.getElementById("group-create-submit").style.display = "none";
    document.getElementById("group-edit-submit").style.display = "none";
    document.getElementById("user-add-submit").style.display = "flex";
    document.getElementById("user-delete-submit").style.display = "none";
    document.getElementById("group-buttons").style.display = "none";
}

function group_edit() {
    document.getElementById("admin-popup").style.height = "60%";
    document.getElementById("admin-popup").style.top = "10%";
    document.getElementById("group-input").style.display = "block";
    document.getElementById("select-user").style.display = "none";
    document.getElementById("group-edit-submit").style.display = "none";
    document.getElementById("user-add-submit").style.display = "none";
    document.getElementById("group-create-submit").style.display = "flex";
    document.getElementById("user-delete-submit").style.display = "none";
    document.getElementById("group-buttons").style.display = "none";
}

function chat_list_back() {
    document.getElementById("chat-box").style.display = "none";
    document.getElementById("search-box").style.display = "flex";
}