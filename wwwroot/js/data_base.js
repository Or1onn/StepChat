let db;
const dbName = "private_keys";

const users = [
    { userId: "tom@gmail.com", key: "sdfdsfdgd/sduygHJKFSD;'LGKJH/234t" },
    { userId: "bob@gmail.com", key: "dsfsdfH/234dfgdfgtw43534t4334sdfs" }
];

function getKey(userId) {
    db.transaction("usersKeys", "readonly").objectStore("usersKeys").get(userId).onsuccess = (event) => {
        return event.target.result.key;
    };


}

function addUser(user) {
    db.transaction("usersKeys", "readwrite").objectStore("usersKeys").put(user);
}

function deleteUser(userId) {
    db.transaction("usersKeys", "readwrite").objectStore("usersKeys").delete(userId);
}

const request = indexedDB.open(dbName, 1);

request.onerror = (event) => {
    // Handle errors.
};

request.onupgradeneeded = (event) => {
    db = event.target.result;

    if (!db.objectStoreNames.contains('usersKeys')) {
        objectStore = db.createObjectStore("usersKeys", { keyPath: "userId" });

        objectStore.createIndex("userId", "userId", { unique: true });

        objectStore.createIndex("key", "key", { unique: true });
    }


    objectStore.transaction.oncomplete = (event) => {
        const usersKeysObjectStore = db.transaction("usersKeys", "readwrite").objectStore("usersKeys");
        users.forEach((customer) => {
            usersKeysObjectStore.add(customer);
        });
    };
};

request.onsuccess = (event) => {
    db = event.target.result;

    let user = {
        userId: 'tom1@gmail.com',
        key: 'dfdfg[sdfdsjurhew/gdoirwueyewrjkmk',
    };
    addUser(user);
};
