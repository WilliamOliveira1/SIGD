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
                    setDataTablePrincipalsFilesList(obj);//Create method to load table with users
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

function setDataTablePrincipalsFilesList(accountList) {
    $('#PrincipalFilesTable').DataTable({
        data: accountList,
        "language": translation,
        autoWidth: false,
        "columnDefs": [
            { "width": "150px", "class": "dt-head-center", "targets": 0 },
            { "width": "190px", "class": "dt-head-center","targets": 1 },
            { "width": "190px", "class": "dt-head-center","targets": 2 },
            { "width": "80px", "class": "dt-head-center","targets": 3 },
            { "width": "190px", "class": "dt-head-center","targets": 4 },
            { "width": "120px", "class": "dt-head-center","targets": 5 },
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
                    return data[0]?.Question ? `<i id="${data.Id}" class="fa-solid fa-person-circle-question fa-xl pointer write-answer"></i>` : `<p class="text-center" title="Não tem pergunta.">---</p>`;
                },
            },
            {
                data: 'PrincipalsFiles',
                render: function (data, type) {
                    return data[0]?.Answer ? `<i id="${data.Id}" class="fa-solid fa-person-circle-question fa-xl pointer"></i>` : `<p class="text-center" title="Não tem resposta ainda.">---</p>`;
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
                    return data[0]?.LastTimeOpened !== '0001-01-01T00:00:00' ? `<span class="ml-3">${data[0]?.LastTimeOpened?.toString()}</span>` : '<p class="text-center">---</p>';
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
    return data[0]?.Status ? `<i class="fa-regular fa-circle-check ml-3 fa-beat fa-xl pointer" title="Documento lido"></i>` : `<i class="fa-solid fa-circle-exclamation ml-3 fa-beat fa-xl pointer" title="Documento não lido"></i>`;
}

$(document).ready(function () {
    if (window.location.href.indexOf("FilesList") > -1) {
        getUsersList();
    }
    $("#embedPDF").attr("hidden", true);
    $("#PrincipalFilesTable").attr("hidden", true);    
});