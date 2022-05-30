// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function createNewUser(username, email) {
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
                //resolve(response);
                window.location.replace(BaseApiUrl() + "/Login/LoginPage");
            })
            .fail(function (response) {
                //reject(response)
                setErrorMessage(response.responseJSON);
            })
    })
}

$('#RegisterSubmit').click(function () {
    let username = document.getElementById('InputUsernameRegister').value;
    let email = document.getElementById('InputEmailRegister').value;

    createNewUser(username, email);
});

function BaseApiUrl() {
    return window.location.origin;
}

function setErrorMessage(message) {
    let timerInterval
    Swal.fire({
        title: 'Erro!',
        text: message,
        icon: 'error',
        confirmButtonText: 'Fechar',
        showCloseButton: true,
        timer: 3500,
        timerProgressBar: true,
        willClose: () => {
            clearInterval(timerInterval)
        }
    })
}

$("#InputUsernameRegister, #InputEmailRegister").on('input', function () {
    registerForm();
});

function registerForm() {
    let username = document.getElementById('InputUsernameRegister').value == null ? "" : document.getElementById('InputUsernameRegister').value;
    let email = document.getElementById('InputEmailRegister').value == null ? "" : document.getElementById('InputEmailRegister').value;
    let isEmailType = isEmail(email);

    if (!username && !email) {
        $("#errorLoginMessage").attr("hidden", false);
        $("#RegisterSubmit").prop("disabled", true);
    }

    if (username.length > 0 || email.length > 0) {
        $("#errorRegisterMessage").attr("hidden", true);
    }

    if (!isEmailType && email.length > 4) {
        $("#errorEmailMessage").attr("hidden", false);
    }
    else if (isEmailType) {
        $("#errorEmailMessage").attr("hidden", true);
        if (username && email) {
            $("#errorRegisterMessage").attr("hidden", true);
            $("#RegisterSubmit").prop("disabled", false);
        }
    }
}

function isEmail(email) {
    let regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

$(document).ready(function () {
    $("#RegisterSubmit").attr("disabled", true);
    $("#errorRegisterMessage").attr("hidden", true);
});