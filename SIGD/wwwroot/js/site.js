function setMessage(message) {
    let timerInterval
    Swal.fire({
        title: 'Sucesso!',
        text: message,
        confirmButtonText: 'Fechar',
        showCloseButton: true,
        timer: 5000,
        timerProgressBar: true,
        willClose: () => {
            clearInterval(timerInterval);
            window.location.reload();
        }
    })
}

function setErrorMessage(message) {
    let timerInterval
    Swal.fire({
        title: 'Erro!',
        text: message,
        icon: 'error',
        confirmButtonText: 'Fechar',
        showCloseButton: true,
        timer: 5000,
        timerProgressBar: true,
        willClose: () => {
            clearInterval(timerInterval);
            window.location.reload();
        }
    })
}

function setMessageWithStatus(message, status) {
    let title = status ? "Sucesso!" : "Erro!";
    let icon = status ? "success" : "error";
    let str = "";
    console.log(typeof message);

    if (typeof message === "object") {
        message.forEach((e) => {
            str = str + e;
        });
    }
    else {
        str = message;
    }


    let timerInterval = str.length * 50 > 5000 ? str.length * 50 : 5000;
    console.log(timerInterval);
    Swal.fire({
        customClass: {
            popup: 'format-pre'
        },
        title: title,
        icon: icon,
        html: '<span>' + str + '</span>',
        customClass: 'swal-wide',
        confirmButtonText: 'Fechar',
        showCloseButton: true,
        timer: timerInterval,
        timerProgressBar: true,
        willClose: () => {
            clearInterval(timerInterval)
            window.location.reload();
        }
    })
}

var translation = {
    "sProcessing": "Processando...",
    "sLengthMenu": "Mostrar _MENU_ registros",
    "sZeroRecords": "Não foi encontrado resultados",
    "sEmptyTable": "Nenhum documento salvo",
    "sInfo": "Mostrando registros de _START_ até _END_ de um total de _TOTAL_ registros",
    "sInfoEmpty": "Mostrando registros de 0 até 0 de um total de 0 registros",
    "sInfoFiltered": "(filtrado de um total de _MAX_ registros)",
    "sInfoPostFix": "",
    "sSearch": "Pesquisar:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Carregando...",
    "oPaginate": {
        "sFirst": "Primeiro",
        "sLast": "Ultimo",
        "sNext": "Seguinte",
        "sPrevious": "Anterior"
    },
    "oAria": {
        "sSortAscending": ": Ativar para ordenar coluna de forma ascendente",
        "sSortDescending": ": Ativar para ordenar coluna de forma descendente"
    }
}

function BaseApiUrl() {
    return window.location.origin;
}

function isEmail(email) {
    let regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}