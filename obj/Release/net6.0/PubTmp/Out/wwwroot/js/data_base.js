let db;
const dbName = "last_messages";

//const users = [
//    { userId: "tom@gmail.com", key: "sdfdsfdgd/sduygHJKFSD;'LGKJH/234t" },
//    { userId: "bob@gmail.com", key: "dsfsdfH/234dfgdfgtw43534t4334sdfs" }
//];

function getKey(userId) {
    db.transaction("userLastMessage", "readonly").objectStore("lastMessage").get(userId).onsuccess = (event) => {
        return event.target.result.key;
    };


}

function addLastMessage(user) {
    db.transaction("userLastMessage", "readwrite").objectStore("lastMessage").put(user);
}

function deleteLastMessage(userId) {
    db.transaction("userLastMessage", "readwrite").objectStore("lastMessage").delete(userId);
}

const request = indexedDB.open(dbName, 1);

request.onerror = (event) => {
    // Handle errors.
};

request.onupgradeneeded = (event) => {
    db = event.target.result;

    if (!db.objectStoreNames.contains('lastMessage')) {
        objectStore = db.createObjectStore("lastMessage", { keyPath: "userId" });

        objectStore.createIndex("userId", "userId", { unique: true });

        objectStore.createIndex("lastMessage", "lastMessage", { unique: false });
    }


    objectStore.transaction.oncomplete = (event) => {
        const lastMessageObjectStore = db.transaction("lastMessage", "readwrite").objectStore("lastMessage");
        users.forEach((customer) => {
            lastMessageObjectStore.add(customer);
        });
    };
};

request.onsuccess = (event) => {
    db = event.target.result;
};
