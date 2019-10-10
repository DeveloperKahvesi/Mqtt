// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function PrependMqttMessage(message, tableId) {
    var data = ParseMqttMessage(message);
    if (!data) {
        return;
    }

    var timeStamp = data.date;
    var signalvalue = data.signal;

    var table = document.getElementById(tableId);

    var row = table.insertRow(1);
    var timeStampCell = row.insertCell(0);
    var signalValueCell = row.insertCell(1);

    var rowClass = "";
    var badgeClass = "";

    if (signalvalue <= 15) {
        rowClass = "alert alert-danger";
        badgeClass = "badge-danger";
    }
    else if (signalvalue >= 15 && signalvalue < 30) {
        rowClass = "alert alert-warning";
        badgeClass = "badge-warning";
    }
    else {
        rowClass = "alert alert-success";
        badgeClass = "badge-success";
    }
    //row.className = rowClass;
    timeStampCell.innerHTML = timeStamp;
    signalValueCell.innerHTML = `<h4><span class="badge ${badgeClass}">${signalvalue}</span></h4>`;

    var cnt = table.rows.length;

    if (cnt > 10) {
        // delete last row
        table.deleteRow(table.rows.length - 1);
    }

}

function ParseMqttMessage(msg) {
    let matchPattern = /(\((?<date>.*)\)):(?<signal>\d+)/;
    let match = matchPattern.exec(msg);

    if (match) {
        return {
            "date": match.groups.date,
            "signal": match.groups.signal
        };
    }
    else {
        return null;
    }
}