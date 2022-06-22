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
        "language": translation,
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

//var translation = {
//    "sProcessing": "Processando...",
//    "sLengthMenu": "Mostrar _MENU_ registros",
//    "sZeroRecords": "Não foi encontrado resultados",
//    "sEmptyTable": "Nenhum documento salvo",
//    "sInfo": "Mostrando registros de _START_ até _END_ de um total de _TOTAL_ registros",
//    "sInfoEmpty": "Mostrando registros de 0 até 0 de um total de 0 registros",
//    "sInfoFiltered": "(filtrado de um total de _MAX_ registros)",
//    "sInfoPostFix": "",
//    "sSearch": "Pesquisar:",
//    "sUrl": "",
//    "sInfoThousands": ",",
//    "sLoadingRecords": "Carregando...",
//    "oPaginate": {
//        "sFirst": "Primeiro",
//        "sLast": "Ultimo",
//        "sNext": "Seguinte",
//        "sPrevious": "Anterior"
//    },
//    "oAria": {
//        "sSortAscending": ": Ativar para ordenar coluna de forma ascendente",
//        "sSortDescending": ": Ativar para ordenar coluna de forma descendente"
//    }
//}