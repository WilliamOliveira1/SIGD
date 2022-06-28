function getPrincipalFilesList(username) {
    let parameters = {
        'username': username,
    };
    let apiPath = BaseApiUrl() + "/api/filemanager/getfilesbyprincipalUsername";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            contentType: 'application/json',
            data: JSON.stringify(parameters)
        })
            .done(function (response) {
                if (response) {
                    let obj = JSON.parse(response)
                    setDataTablePrincipalsFilesList(obj);//Create method to load table with users
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function changeReadingStatus(filename) {
    let parameters = {
        'filename': filename,
    };
    let apiPath = BaseApiUrl() + "/api/filemanager/changeReadingStatus";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            contentType: 'application/json',
            data: JSON.stringify(parameters)
        })
            .done(function (response) {
                if (response) {
                    $('#exampleModalLong').modal('hide');
                    setMessage(response);
                }
            })
            .fail(function (response) {
                $('#exampleModalLong').modal('hide');
                setErrorMessage(response.responseJSON);
            })
    })
}

function sendFileQuestion(filename, message) {
    let parameters = {
        'filename': filename,
        'message': message
    };
    let apiPath = BaseApiUrl() + "/api/filemanager/sendFileQuestion";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            contentType: 'application/json',
            data: JSON.stringify(parameters)
        })
            .done(function (response) {
                if (response) {
                    $('#exampleModalMessageLong').modal('hide');
                    setMessage(response);
                }
            })
            .fail(function (response) {
                $('#exampleModalMessageLong').modal('hide');
                setErrorMessage(response.responseJSON);
            })
    })
}

function BaseApiUrl() {
    return window.location.origin;
}

function setDataTablePrincipalsFilesList(accountList) {
    let addNumber = 0;
    table = $('#PrincipalFilesTableData').DataTable({
        data: accountList,
        "language": translation,
        autoWidth: false,
        "columnDefs": [
            { "width": "350px", "class": "dt-head-center", "targets": 0 },
            { "width": "80px", "class": "dt-head-center", "targets": 1 },
            { "width": "80px", "class": "dt-head-center", "targets": 2 },
            { "width": "80px", "class": "dt-head-center", "targets": 3 },
            { "width": "190px", "class": "dt-head-center", "targets": 4 },
            { "width": "120px", "class": "dt-head-center", "targets": 5 },
        ],
        responsive: true,
        "ordering": false,
        columns: [
            {
                data: 'FileName',
                render: function (data, type) {
                    return type === 'display' ? '<span class="ml-3">' + data?.toString() + '</span>' : data;
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    let renderCell = "";
                    if (data[0]?.Question) {
                        renderCell = `<i id="${data[0].Id}" class="fa-solid fa-person-circle-question fa-xl pointer write-answer edit-question ml-4"></i>`;
                    }
                    else {
                        renderCell = `<i id="question${addNumber}" class="fa-solid fa-clipboard-question fa-xl pointer ml-4 open-question"></i>`;
                    }                    
                    addNumber++;
                    return renderCell;
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    return editAnswerCell(data);
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    return setStatusIcon(data);
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    if (data[0]?.LastTimeOpened !== '0001-01-01T00:00:00') {
                        var date = new Date(data[0]?.LastTimeOpened);
                        var dateString =
                            date.getDate() + "/" +
                            ("0" + (date.getUTCMonth() + 1)).slice(-2) + "/" +
                            (date.getUTCFullYear()) + " " +
                            ("0" + date.getHours()).slice(-2) + ":" +
                            ("0" + date.getMinutes()).slice(-2);
                        return `<span class="ml-3">${dateString.toString()}</span>`
                    }
                    else {
                        return '<p class="text-center">---</p>';
                    }                                         
                },
            },
            {
                data: 'FileName',
                render: function (data, type) {
                    return setIconAction(data);
                },
            },
        ]
    });
}

function editAnswerCell(data) {
    if (data) {
        if (data[0].Question) {
            return data[0]?.Answer ? `<i id="${data.Id}" class="fa-regular fa-square-check fa-xl pointer ml-3 open-answer" title="Abrir a resposta."></i>` : `<i id="${data.Id}" class="fa-regular fa-circle-question fa-xl pointer ml-3 send-answer" title="Enviar resposta"></i>`;
        }
        else {
            return data[0]?.Answer ? `<i id="${data.Id}" class="fa-solid fa-person-circle-question fa-xl pointer"></i>` : `<p class="text-center" title="Não tem resposta ainda.">---</p>`;
        }
    }
}

$('#tableElement tbody').on('click', '.open-answer', (e) => {
    $("#modalFile").empty();
    $("#exampleModalLongTitle").empty();
    let id = e.currentTarget.attributes[0].nodeValue;
    let row = $(`#${id}`).parents('tr')[0];
    let rowData = table.row(row).data();
    let questionRow = rowData.PrincipalsFiles[0].Answer.split("<>:")
    let questionLines = questionRow[1].split("-newBreakline-");
    let questionMessage = "";
    questionLines.forEach((e) => {
        if (e) {
            questionMessage = questionMessage + `<div><span id="editQuestionModal">${e}</span></div>`;
        }
    });
    let questionDate = `<div class="mt-4"><span id="editQuestionModal">Resposta salva na data: ${questionRow[2].toString().split("Last Sync: ").pop()}</span></div>`;
    $("#exampleModalLongTitle").text(questionRow[0]);
    $("#modalFile").append(questionMessage);
    $("#modalFile").append(questionDate);
    $("#modalButton").click();
});

$('#tableElement tbody').on('click', '.download-doc', (e) => {
    let filename = e.currentTarget.attributes[0].nodeValue;
    downloadFile(filename);
});

$('#modalQAFooter').on('click', '#sendQuestion', (e) => {
    let modalTitle = $("#ModalQALongTitle")[0].childNodes[0].nodeValue.split(" - ");
    let filename = modalTitle[1];
    let messageTextArea = $("#inputMessage").val();
    let message = "";
    var eachLine = messageTextArea.split('\n');
    eachLine.forEach((e) => {
        if (e) {
            message = message ? `${message}-newBreakline-${e}` : `${message}${e}`;
        }
    });
    let messageTitle = $("#inputMessageTitle").val();
    var currentdate = new Date();
    let hours = currentdate.getHours().lenght === 1 ? `0${currentdate.getHours()}` : `${currentdate.getHours()}:`;
    let minutes = currentdate.getMinutes().lenght === 1 ? `0${currentdate.getMinutes()}` : `${currentdate.getMinutes()}`;
    var datetime = currentdate.getDate() + "/"
        + (currentdate.getMonth() + 1) + "/"
        + currentdate.getFullYear() + " - "
        + hours +
        + minutes;
    let question = `${messageTitle}<>:${message}<>:${datetime.toString()}`
    sendFileQuestion(filename, question);
    e.preventDefault();
});

$('#tableElement tbody').on('click', '.write-answer', (e) => {
    let filename = e.currentTarget.attributes[0];
});

$('#tableElement tbody').on('click', '.open-doc', (e) => {
    let filename = e.currentTarget.attributes[2].nodeValue;
    let test = filename.includes("pdf");
    let id = e.currentTarget.attributes[0].nodeValue;
    let row = $(`#${id}`).parents('tr')[0];
    let rowData = table.row(row).data();
    let status = rowData.PrincipalsFiles[0].Status
    if (test) {
        openPDFFile(filename, status);
    }
    else {
        openFile(filename, status);
    }
});

$('#tableElement tbody').on('click', '.open-question', (e) => {    
    $("#modalMessageButton").click();
    let buttonConfirm = `<button id="sendQuestion" type="button" class="btn btn-primary">Enviar pergunta</button>`;
    let id = e.currentTarget.attributes[0].nodeValue;
    let row = $(`#${id}`).parents('tr')[0];
    let rowData = table.row(row).data();
    $("#modalQAFooter").append(buttonConfirm);
    $("#ModalQALongTitle").empty();
    $("#ModalQALongTitle").text(`Enviar pergunta arquivo - ${rowData.FileName}`);
});

$('#tableElement tbody').on('click', '.edit-question', (e) => {
    $("#modalFile").empty();
    $("#exampleModalLongTitle").empty();
    let id = e.currentTarget.attributes[0].nodeValue;
    let row = $(`#${id}`).parents('tr')[0];
    let rowData = table.row(row).data();
    let questionRow = rowData.PrincipalsFiles[0].Question.split("<>:")
    let questionLines = questionRow[1].split("-newBreakline-");
    let questionMessage = "";
    questionLines.forEach((e) => {
        if (e) {
            questionMessage = questionMessage + `<div><span id="editQuestionModal">${e}</span></div>`;
        }
    });    
    let questionDate = `<div class="mt-4"><span id="editQuestionModal">Pergunta salva na data: ${questionRow[2].toString().split("Last Sync: ").pop()}</span></div>`;
    $("#exampleModalLongTitle").text(questionRow[0]);
    $("#modalFile").append(questionMessage);
    $("#modalFile").append(questionDate);
    $("#modalButton").click();
});

$('#exampleModalLong').on('click', '#confirmReadingStatus', (e) => {
    let modalTitle = $("#exampleModalLongTitle");
    let filename = modalTitle[0].innerHTML;
    changeReadingStatus(filename)
});

function downloadFile(filename) {
    let parameters = {
        'filename': filename,
    };

    let path = BaseApiUrl() + `/api/filemanager/download`;
    return new Promise(function (resolve, reject) {
        $.post({
            url: path,
            contentType: 'application/json',
            data: JSON.stringify(parameters),
            xhrFields: {
                responseType: 'blob'
            }
        })
            .done(function (response) {
                if (response) {
                    let fileName = parameters.filename;
                    let file1 = blobToFile(response, fileName)
                    var a = window.document.createElement('a');
                    a.href = window.URL.createObjectURL(file1);
                    a.download = file1.name;
                    // Append anchor to body.
                    document.body.appendChild(a);
                    a.click();
                    // Remove anchor from body
                    document.body.removeChild(a);
                }
                else {

                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function openPDFFile(filename, status) {
    let parameters = {
        'filename': filename,
    };

    let path = BaseApiUrl() + `/api/filemanager/download`;
    return new Promise(function (resolve, reject) {
        $.post({
            url: path,
            contentType: 'application/json',
            data: JSON.stringify(parameters),
            xhrFields: {
                responseType: 'blob'
            }
        })
            .done(function (response) {
                if (response) {
                    $("#modalFile").empty();
                    $("#exampleModalLongTitle").empty();
                    $("#modalFile").append(`<embed id="embedPDF" width="750" height="1000">`);
                    let fileName = parameters.filename;
                    let type = response.type;
                    let file1 = blobToFile(response, fileName)
                    if (type === "application/pdf") {
                        var fileURL = URL.createObjectURL(file1);
                        $("#embedPDF").attr("src", fileURL);
                        $("#embedPDF").attr("hidden", false);
                    }
                    $("#exampleModalLongTitle").html(filename);
                    if (!status) {
                        $("#modalFileFooter").append(`<button id="confirmReadingStatus" type="button" class="btn btn-primary">Confirmar leitura</button>`);
                    }
                    $("#modalButton").click();
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

//Execute hen modal is closed by any means
$('#exampleModalLong').on('hide.bs.modal', (e) => {
    $("#confirmReadingStatus").remove();
});

$('#exampleModalMessageLong').on('hide.bs.modal', (e) => {
    $("#sendQuestion").remove();
});

function openFile(filename, status) {
    let parameters = {
        'filename': filename,
    };

    let path = BaseApiUrl() + `/api/filemanager/download`;
    return new Promise(function (resolve, reject) {
        $.post({
            url: path,
            contentType: 'application/json',
            data: JSON.stringify(parameters)
        })
            .done(function (response) {
                if (response) {
                    $("#modalFile").empty();
                    $("#exampleModalLongTitle").empty();
                    $("#embedPDF").attr("hidden", true);
                    response = response.replaceAll("<", "");
                    response = response.replaceAll(">", "");
                    response = response.replaceAll("</", "");
                    response = response.replaceAll("/", "");
                    $("#modalFile").append(response);
                    $("#exampleModalLongTitle").html(filename);
                    if (!status) {
                        $("#modalFileFooter").append(`<button id="confirmReadingStatus" type="button" class="btn btn-primary">Confirmar leitura</button>`);
                    }
                    $("#modalButton").click();
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function blobToFile(blob, fileName) {
    return new File([blob], fileName, { lastModified: new Date().getTime(), type: blob.type })
}

$('#principalsFilesSelect').click((e) => {
    let conceptName = $('#principalsList').find(":selected");
    let optionSelected = conceptName[0].innerHTML;
    let isEmailSelected = isEmail(optionSelected);
    $("#PrincipalFilesTable").DataTable().clear().destroy();
    $("#usersManagedTable").DataTable().clear().destroy();
    if (isEmailSelected) {
        $("#PrincipalFilesTable").attr("hidden", false);
        $("#usersManagedTable").attr("hidden", true);
        getPrincipalFilesList(optionSelected);
    }
    else {
        $("#PrincipalFilesTable").attr("hidden", true);
        $("#usersManagedTable").attr("hidden", false);
        getUsersList();
    }
});

function isEmail(email) {
    let regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

function setIconAction(data) {
    let iconCell = "";
    let fileWithoutExt = data.toString().split(".");
    let openIcon = `<i id="${fileWithoutExt[0]}" action="open" data-name="${data.toString()}" data-toggle="modal" data-target="#exampleModalLong" class="ml-2 fa-solid fa-book-open fa-xl open-doc pointer" title="Abrir documento"></i>`;
    let downloadIcon = `<i id="${data.toString()}" action="download" class="ml-2 fa-solid fa-download fa-xl download-doc pb-2 pointer" title="Download do documento"></i>`;
    iconCell = iconCell + openIcon;
    iconCell = iconCell + downloadIcon;

    return iconCell;
}

function setStatusIcon(data) {
    return data[0]?.Status ? `<i class="fa-regular fa-circle-check ml-3 fa-xl pointer" title="Documento lido"></i>` : `<i class="fa-solid fa-circle-exclamation ml-3 fa-beat fa-xl pointer" title="Documento não lido"></i>`;
}

$(document).ready(function () {
    if (window.location.href.indexOf("FilesList") > -1) {
        let username = $('.username').attr('id');
        getPrincipalFilesList(username);
    }
    $("#embedPDF").attr("hidden", true);    
});
var table = null;