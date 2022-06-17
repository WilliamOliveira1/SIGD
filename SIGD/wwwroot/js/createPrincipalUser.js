function getUsersListManaged() {
    let apiPath = BaseApiUrl() + "/api/usersmanaged/listmanaged";

    return new Promise(function (resolve, reject) {
        $.get({
            url: apiPath,
            contentType: 'application/json'
        })
            .done(function (response) {
                if (response) {
                    setDataTable(response);//Create method to load table with users
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function BaseApiUrl() {
    return window.location.origin;
}

function setDataTable(accountList) {
    $('#usersManagedTable').DataTable({
        data: accountList,
        autoWidth: false,
        columns: [
            {
                data: 'userName',
                render: function (data, type) {
                    return type === 'display' ? '<span class="ml-3">' + data.toString() + '</span>' : data;
                },
            },
            {
                data: 'email',
                render: function (data, type) {
                    return type === 'display' ? '<span class="ml-3">' + data.toString() + '</span>' : data;
                },
            },
            {
                data: 'isActivated',
                render: function (data, type) {
                    if (data.toString() === 'true') {
                        return type === 'display' ? '<span class="ml-3">' + 'Ativada' + '</span>' : data;
                    }
                    else {
                        return type === 'display' ? '<span class="ml-3">' + "Não ativada" + '</span>' : data;
                    }                    
                },
            }
        ],
    });    
}

$(document).ready(function () {
    if (window.location.href.indexOf("CreateUser") > -1) {
        getUsersListManaged();
    }
});