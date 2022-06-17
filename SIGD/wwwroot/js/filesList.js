function getUsersList() {
    let apiPath = BaseApiUrl() + "/api/filemanager/getfilesbyuser";

    return new Promise(function (resolve, reject) {
        $.get({
            url: apiPath,
            contentType: 'application/json'
        })
            .done(function (response) {
                if (response) {
                    setDataTableFileList(response);//Create method to load table with users
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

function setDataTableFileList(accountList) {
    $('#usersManagedTable').DataTable({
        data: accountList,
        autoWidth: false,
        columns: [
            {
                data: 'fileName',
                render: function (data, type) {
                    return type === 'display' ? '<span class="ml-3">' + data?.toString() + '</span>' : data;
                },
            },
            {
                data: 'fileName',
                render: function (data, type) {
                    return type === 'display' ? `<a id="${data?.toString()}" href="#" action="open" data-toggle="modal" data-target="#exampleModalLong" class="pe-auto open-doc">Abrir documento</a>` : data;
                },
            },
            {
                data: 'fileName',
                render: function (data, type) {
                    return type === 'display' ? `<a id="${data?.toString()}" href="#" action="download" class="pe-auto download-doc">Download</a>` : data;
                },
            },
            {
                data: 'listOfReaders',
                render: function (data, type) {
                    let i = 1;
                    let select = `<select id="readerList" class="mt-2">`;
                    let option = "";
                    data.forEach((e) => {                        
                        option = `<option value="position${i}">${e}</option>`
                        select = select + option;
                        option = "";
                        i++;
                    });
                    select.concat("</select>");
                    return type === 'display' ? select : data;
                },
            }
        ]
    });    
}

$('#usersManagedTable tbody').on('click', 'a.download-doc', (e) => {
    console.log(e);
    let filename = e.currentTarget.attributes[0].nodeValue;
    console.log(filename);
    downloadFile(filename);
})

$('#usersManagedTable tbody').on('click', 'a.open-doc', (e) => {
    let filename = e.currentTarget.attributes[0].nodeValue;
    let test = filename.includes("pdf");
    if (test) {
        openPDFFile(filename);
    }
    else {
        openFile(filename);
    }    
})

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

$(document).ready(function () {
    if (window.location.href.indexOf("FilesList") > -1) {
        getUsersList();
    }
    $("#embedPDF").attr("hidden", true);
});