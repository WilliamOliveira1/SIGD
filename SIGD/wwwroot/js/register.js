// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function clientList(username, email) {
    let parameters = {
        'username': username,
        'email': email
    };

    return new Promise(function (resolve, reject) {
        $.post({
            url: BaseApiUrl() + "/api/register/newregister",
            data: JSON.stringify(parameters),
            dataType: 'json',
            contentType: 'application/json'
        })
            .done(function (response) {
                resolve(response);
                alertResponse(response);
            })
            .fail(function (response) {
                reject(response)
            })
    })
}

$('#RegisterSubmit').click(function () {
    let username = document.getElementById('InputUser').value;
    let email = document.getElementById('InputEmail').value;

    clientList(username, email);
});

function BaseApiUrl() {
    return window.location.origin;
}