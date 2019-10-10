"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/mqtt").build();

connection.on("ReceiveMessage", function (msg) {
    PrependMqttMessage(msg, "signal-table");
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});

