function getUsersList() {    
    let apiPath = BaseApiUrl() + "/api/filemanager/getfilesbyuser";

    return new Promise(function (resolve, reject) {
        $.get({
            url: apiPath,
            contentType: 'application/json'
        })
            .done(function (response) {
                if (response) {
                    let obj = JSON.parse(response)
                    setDataTableFileList(obj);//Create method to load table with users
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function getPrincipalFilesList(principalEmail) {
    let parameters = {
        'useremail': principalEmail,
    };
    let apiPath = BaseApiUrl() + "/api/filemanager/getfilesbyprincipal";

    return new Promise(function (resolve, reject) {
        $.post({
            url: apiPath,
            contentType: 'application/json',
            data: JSON.stringify(parameters)
        })
            .done(function (response) {
                if (response) {
                    let obj = JSON.parse(response)
                    setDataTablePrincipalsFilesList(obj, principalEmail);//Create method to load table with users
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function setDataTableFileList(accountList) {
    $('#usersManagedTable').removeAttr('width').DataTable({
        data: accountList,
        responsive: true,
        "language": translation,
        "columnDefs": [
            { "class": "dt-head-center", "targets": 0 },
            { "width": "200px", "class": "dt-head-center", "targets": 1 },
            { "class": "dt-head-center", "targets": 2 }
        ],        
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
                    let i = 1;
                    let select = `<select id="readerList" class="mt-2">`;
                    let option = "";
                    data.forEach((e) => {
                        option = `<option value="position${i}">${e.PrincipalEmail}</option>`
                        select = select + option;
                        option = "";
                        i++;
                    });
                    select.concat("</select>");
                    return type === 'display' ? select : data;
                },
            },
            {
                data: 'FileName',
                render: function (data, type) {
                    return setIconAction(data);
                },
            }
        ]
    });    
}

function setDataTablePrincipalsFilesList(accountList, principalEmail) {
    let addNumber = 0;
    table = $('#PrincipalFilesTable').DataTable({
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
                    data = data.find(x => x.PrincipalEmail == principalEmail);
                    let renderCell = "";
                    if (data?.Question) {
                        renderCell = `<i id="${data.Id}" class="fa-solid fa-person-circle-question fa-xl pointer write-answer show-question ml-4" title="Abrir pergunta"></i>`;
                    }
                    else {
                        renderCell = `<i id="question${addNumber}" class="fa-solid fa-clipboard-question fa-xl pointer ml-4" title="Não foi feito pergunta ainda."></i>`;
                    }
                    addNumber++;
                    return renderCell;
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    data = data.find(x => x.PrincipalEmail == principalEmail);
                    return editAnswerCell(data);
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    data = data.find(x => x.PrincipalEmail == principalEmail);
                    return setStatusIcon(data);
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    data = data.find(x => x.PrincipalEmail == principalEmail);
                    if (data?.LastTimeOpened !== '0001-01-01T00:00:00') {
                        var date = new Date(data?.LastTimeOpened);
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

$('#tableElement tbody').on('click', '.open-answer', (e) => {
    $("#modalFileSup").empty();
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
    $("#modalFileSup").append(questionMessage);
    $("#modalFileSup").append(questionDate);
    $("#modalButton").click();
});

$('#tableElement tbody').on('click', '.send-answer', (e) => {
    $("#modalMessageButton").click();
    let buttonConfirm = `<button id="sendAnswer" type="button" class="btn btn-primary">Enviar resposta</button>`;
    let id = e.currentTarget.attributes[0].nodeValue;
    let row = $(`#${id}`).parents('tr')[0];
    let rowData = table.row(row).data();
    $("#modalAnswerFooter").append(buttonConfirm);
    $("#ModalQALongTitle").empty();
    $("#ModalQALongTitle").text(`Enviar resposta arquivo - ${rowData.FileName}`);
});

$('#modalAnswerFooter').on('click', '#sendAnswer', (e) => {
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
    sendFileAnswer(filename, question);
    e.preventDefault();
});

function sendFileAnswer(filename, message) {
    let parameters = {
        'filename': filename,
        'message': message
    };
    let apiPath = BaseApiUrl() + "/api/filemanager/sendFileAnswer";

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

$('#exampleModalMessageLong').on('hide.bs.modal', (e) => {
    $("#sendQuestion").remove();
});

function editAnswerCell(data) {
    if (data) {
        if (data.Question) {
            return data?.Answer ? `<i id="${data.Id}" class="fa-regular fa-square-check fa-xl pointer ml-3 open-answer" title="Abrir a resposta."></i>` : `<i id="${data.Id}" class="fa-regular fa-circle-question fa-xl pointer ml-3 send-answer" title="Enviar resposta"></i>`;
        }
        else {
            return data?.Answer ? `<i id="${data.Id}" class="fa-solid fa-person-circle-question fa-xl pointer"></i>` : `<p class="text-center" title="Não tem resposta ainda.">---</p>`;
        }
    }
}

$('#tableElement tbody').on('click', '.show-question', (e) => {
    $("#modalFileSup").empty();
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
    $("#modalFileSup").append(questionMessage);
    $("#modalFileSup").append(questionDate);
    $("#modalButton").click();
});

$('#tableElement tbody').on('click', '.download-doc', (e) => {
    let filename = e.currentTarget.attributes[0].nodeValue;
    downloadFile(filename);
});

$('#tableElement tbody').on('click', '.write-answer', (e) => {
    let filename = e.currentTarget.attributes[0];
});

$('#tableElement tbody').on('click', '.open-doc', (e) => {
    let filename = e.currentTarget.attributes[0].nodeValue;
    let test = filename.includes("pdf");
    if (test) {
        openPDFFile(filename);
    }
    else {
        openFile(filename);
    }
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

function openPDFFile(filename) {
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
                    $(".modal-body").empty();
                    $(".modal-body").append(`<embed id="embedPDF" width="750" height="1000">`);
                    let fileName = parameters.filename;
                    let type = response.type;
                    let file1 = blobToFile(response, fileName)
                    if (type === "application/pdf") {
                        var fileURL = URL.createObjectURL(file1);
                        $("#embedPDF").attr("src", fileURL);                        
                        $("#embedPDF").attr("hidden", false);
                    }
                    $("#exampleModalLongTitle").html(filename);
                    $("#modalButton").click();
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function openFile(filename) {
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
                    $(".modal-body").empty();
                    $("#embedPDF").attr("hidden", true);
                    response = response.replaceAll("<", "");
                    response = response.replaceAll(">", "");
                    response = response.replaceAll("</", "");
                    response = response.replaceAll("/", "");
                    $(".modal-body").append(response);
                    $("#exampleModalLongTitle").html(filename);
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

function setIconAction(data) {
    let iconCell = "";
    let openIcon = `<i id="${data.toString()}" action="open" data-toggle="modal" data-target="#exampleModalLong" class="ml-2 fa-solid fa-book-open fa-xl open-doc pointer" title="Abrir documento"></i>`;
    let downloadIcon = `<i id="${data.toString()}" action="download" class="ml-2 fa-solid fa-download fa-xl download-doc pb-2 pointer" title="Download do documento"></i>`;
    iconCell = iconCell + openIcon;
    iconCell = iconCell + downloadIcon;

    return iconCell;
}

function setStatusIcon(data) {
    return data?.Status ? `<i class="fa-regular fa-circle-check ml-3 fa-xl pointer" title="Documento lido"></i>` : `<i class="fa-solid fa-circle-exclamation ml-3 fa-beat fa-xl pointer" title="Documento não lido"></i>`;
}

$(document).ready(function () {
    if (window.location.href.indexOf("FilesList") > -1) {
        getUsersList();
    }
    $("#embedPDF").attr("hidden", true);
    $("#PrincipalFilesTable").attr("hidden", true);    
});