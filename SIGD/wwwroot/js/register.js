// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function clientList(username, email) {
    let parameters = {
        'username': username,
        'email': email
    };
    let apiPath = BaseApiUrl() + "/api/register/registernew";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            data: JSON.stringify(parameters),
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

function list() {
    return new Promise(function (resolve, reject) {
        $.get({
            url: BaseApiUrl() + "/api/register/list",
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
    //list();
});

function BaseApiUrl() {
    return window.location.origin;
}