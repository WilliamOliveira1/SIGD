// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function login(username, email, password) {
    let parameters = {
        'username': username,
        'email': email,
        'password': password
    };
    let apiPath = BaseApiUrl() + "/api/register/login";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            data: JSON.stringify(parameters),
            contentType: 'application/json'
        })
            .done(function (response) {                
                if (response) {
                    $("#changePassword").attr("hidden", false);
                    $("#loginForm").attr("hidden", true);
                    setMessage(response);
                }
                else {
                    $(location).attr('href', '../../');
                }
                //resolve(response);
                //alertResponse(response);
            })
            .fail(function (response) {
                //reject(response)
                setErrorMessage(response.responseJSON);
            })
    })
}

function logout() {
    let apiPath = BaseApiUrl() + "/api/register/logout";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            contentType: 'application/json'
        })
            .done(function (response) {
                if (response) {
                    $("#changePassword").attr("hidden", false);
                    $("#loginForm").attr("hidden", true);
                    setMessage(response);
                }
                else {
                    $(location).attr('href', '../../Home');
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function changePassword(oldPassword, password, email, username) {
    let parameters = {
        'password': password,
        'oldpassword': oldPassword,
        'email': email,
        'username': username
    };
    let apiPath = BaseApiUrl() + "/api/register/changepassword";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            data: JSON.stringify(parameters),
            contentType: 'application/json'
        })
            .done(function (response) {
                if (response) {
                    setMessage(response)
                }
                else {
                    location.reload();
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

$('#LoginSubmit').click(function () {
    let username = document.getElementById('InputUsername').value == null ? "" : document.getElementById('InputUsername').value;
    let email = document.getElementById('InputEmail').value == null ? "" : document.getElementById('InputEmail').value;
    let password = document.getElementById('InputPassword').value == null ? "" : document.getElementById('InputPassword').value;

    login(username, email, password);
});

$('#logoutButton').click(function () {  
    logout();
});

$('#ChangePasswordSubmit').click(function () {
    let email = document.getElementById('InputEmail').value == null ? "" : document.getElementById('InputEmail').value;
    let username = document.getElementById('InputUsername').value == null ? "" : document.getElementById('InputUsername').value;
    let firstAccessPassword = document.getElementById('InputFirstAccessPassword').value == null ? "" : document.getElementById('InputFirstAccessPassword').value;
    let newPassword = document.getElementById('InputNewPassword').value == null ? "" : document.getElementById('InputNewPassword').value;

    changePassword(firstAccessPassword, newPassword, email, username);
});

$("#InputCheckNewPassword, #InputNewPassword, #InputFirstAccessPassword").on('input', function () {
    changePasswordForm();
});

$("#InputUsername,#InputEmail,#InputPassword").on('input', function () {
    loginForm();
});

function changePasswordForm() {
    let firstAccessPassword = document.getElementById('InputFirstAccessPassword').value == null ? "" : document.getElementById('InputFirstAccessPassword').value;
    let newPassword = document.getElementById('InputNewPassword').value == null ? "" : document.getElementById('InputNewPassword').value;
    let checkNewpassword = document.getElementById('InputCheckNewPassword').value == null ? "" : document.getElementById('InputCheckNewPassword').value;

    if (newPassword !== checkNewpassword) {
        $("#errorPasswordMessage").attr("hidden", false);
    }
    else if (newPassword === checkNewpassword) {
        $("#errorPasswordMessage").attr("hidden", true);
    }

    if (firstAccessPassword && newPassword && checkNewpassword) {
        $("#ChangePasswordSubmit").prop("disabled", false);
        $("#errorChangeMessage").prop("disabled", true);
    }
    else if (!firstAccessPassword && !newPassword && !checkNewpassword) {
        $("#ChangePasswordSubmit").prop("disabled", false);
        $("#errorChangeMessage").prop("disabled", false);
    }    
}

function loginForm() {
    let username = document.getElementById('InputUsername').value == null ? "" : document.getElementById('InputUsername').value;
    let email = document.getElementById('InputEmail').value == null ? "" : document.getElementById('InputEmail').value;
    let password = document.getElementById('InputPassword').value == null ? "" : document.getElementById('InputPassword').value;;
    let isEmailType = isEmail(email);

    if (!username && !email && !password) {
        $("#errorLoginMessage").attr("hidden", false);
        $("#LoginSubmit").prop("disabled", true);
    }

    if (username.length > 0 && email.length > 0 || password.length > 0) {
        $("#errorLoginMessage").attr("hidden", true);
    }

    if (!isEmailType && email.length > 4) {
        $("#errorEmailMessage").attr("hidden", false);
    }
    else if (isEmailType) {
        $("#errorEmailMessage").attr("hidden", true);
        if (password && password.length >= 8) {
            $("#errorLoginMessage").attr("hidden", true);
            $("#LoginSubmit").prop("disabled", false);
        }
    }
    else if (username) {
        if (password && password.length >= 8) {
            $("#errorLoginMessage").attr("hidden", true);
            $("#LoginSubmit").prop("disabled", false);
        }
    }
}

$(document).ready(function () {
    $("#LoginSubmit").attr("disabled", true);
    $("#ChangePasswordSubmit").attr("disabled", true);
    $("#errorPasswordMessage").attr("hidden", true);
    $("#errorLoginMessage").attr("hidden", true);
    $("#errorEmailMessage").attr("hidden", true);
    $("#changePassword").attr("hidden", true);
    $("#errorChangeMessage").prop("disabled", true);
});