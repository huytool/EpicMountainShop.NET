"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
$.get('/ServiceRequests/ServiceRequest/ServiceRequestMessages?serviceRequestId=' + $('#ServiceRequest_RowKey').val(),
    function (data, status) {
        addMessagesToList(data);
    });
function scrollToLatestMessages() {
    $('.messages').animate({ scrollTop: 10000 }, 'normal');
};
function addMessagesToList(messages) {
    if (messages.length === 0) {
        $('.noMessages').removeClass('hide');
    }
    $.each(messages, function (index) {
        var message = messages[index];
        displayMessage(message);
    });
    scrollToLatestMessages();
};
function addMessage(message) {
    if (message.PartitionKey !== $('#ServiceRequest_RowKey').val()) {
        return;
    }
    if (message !== null) {
        $('.noMessages').addClass('hide');
    }
    displayMessage(message);
    scrollToLatestMessages();
};
function displayMessage(message) {
    var isCustomer = $("#hdnCustomerEmail").val() === message.FromEmail ? 'bluelighten - 1' : 'teal lighten - 2';
    $('#messagesList').append(
        '<li class="card-panel ' + isCustomer + ' white-text padding-10px">' +
        '<div class="col s12 padding-0px">' +
        '<div class="col s8 padding-0px"><b>' + message.FromDisplayName + '</b ></div > ' +

        '<div class="col s4 padding-0px font-size-12px right-align">' + (new

            Date(message.MessageDate)).toLocaleString() + '</div>' +

        '</div><br>' + message.Message + '</li>'
    );
};
connection.on("ReceiveMessage", addMessage);

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
$('#txtMessage').on('keypress',function (e) {
    var key = e.which;
    if (key == 13) {
        var message = new Object();
        message.Message = $('#txtMessage').val();
        message.PartitionKey = $('#ServiceRequest_RowKey').val();
        $.post('/ServiceRequests/ServiceRequest/CreateServiceRequestMessage',
            { message: message },
            function (data, status, xhr) {

                if (data) {
                    $('.noMessages').addClass('hide');
                    $('#txtMessage').val('');

                }
            });
        scrollToLatestMessages();
    }
});
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});