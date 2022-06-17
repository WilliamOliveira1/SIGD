$(document).ready(function () {
    let apiPath = BaseApiUrl() + "/api/filemanager/savefiles";

    function BaseApiUrl() {
        return window.location.origin;
    }

    $('#fileSubmit').click((e) => {

        let myFile = $('#fileTest').prop('files');
        const fd = new FormData();
        let $emails = [];
        let listTag = $("#listSelected");
        listTag[0].childNodes.forEach((e) => {
            if (e.nextSibling) {
                $emails.push(e.nextSibling.innerText);
            }                        
        });

        // add all selected files
        for (var i = 0; i < myFile.length; i++) {
            fd.append("test", myFile[i], myFile[i].name);
            console.log(myFile[i].name);
        }
        console.log(fd.getAll('test'));

        for (var i = 0; i < $emails.length; i++) {
            try {
                fd.append("emails", $emails[i]);
            }
            catch (err) {
                console.log(err.message);
            }            
            console.log(fd.getAll('emails'));
        }
        
        fetch(
            apiPath,
            {
                method: "POST",
                body: fd
            })
            .then((response) => {
                return response.json().then((data) => {
                    console.log(data);
                    return data;
                }).catch((err) => {
                    console.log(err);
                })
            })
            .then((responseJson) => {
                // Do something with the response
            })
            .catch((error) => {
                console.log(error)
            });
        e.preventDefault();
    });


    function getPrincipals(username) {
        let path = BaseApiUrl() + "/api/usersmanaged/listmanaged";
        return new Promise(function (resolve, reject) {
            $.get({
                url: path,
                contentType: 'application/json'
            })
                .done(function (response) {
                    if (response) {
                        response.forEach(function (e, i) {
                            $('#principalsList').append($('<option></option>').val(e.id).text(e.email));
                        });
                    }
                    else {

                    }
                })
                .fail(function (response) {
                    setErrorMessage(response.responseJSON);
                })
        })
    }

    getPrincipals();

    $('#principalsSelect').click((e) => {
        let conceptName = $('#principalsList').find(":selected");
        let email = conceptName[0].label;
        let value = conceptName[0].attributes[0].value;        
        if (value === "0") {
            $("#principalsList option").each(function () {
                if (this.value === "0") {
                    $(`#principalsList option[value=${this.value}]`).remove();
                }
                else
                {
                    $(`#principalsList option[value=${this.value}]`).remove();
                    $("#listSelected").append(`<div><span id="${this.value}" class="principalEmail">${this.innerHTML}</span></div>`);
                    havePrincipal = true;
                }
            });
        }
        else {
            $(`#principalsList option[value=${value}]`).remove();
            $("#listSelected").append(`<h6 id="${value}">${email}</h6>`);
            havePrincipal = true;
        }

        $("#selectedListTitle").empty();
        $("#selectedListTitle").append("<h6>Escolas selecionadas:</h6>");

        if (haveFile && havePrincipal) {
            $("#fileSubmit").prop('disabled', false);
        }

        let listCount = $('#principalsList option').length;
        if (listCount < 1) {
            $("#schoolsAdd").attr("hidden", true);
        }
        else if (listCount === 1) {
            $("#principalsList option").each(function () {
                let getLastOption = $(this).val();
                if (getLastOption == "0") {
                    $("#schoolsAdd").attr("hidden", true);
                }
            });
        }

        e.preventDefault();
    });

    $('#inputFileCard').change((e) =>
    {
        let myFile = $('#fileTest').prop('files');

        if (myFile.length > 0) {
            haveFile = true;
        }
        else {
            haveFile = false;
            $("#fileSubmit").prop('disabled', true);
        }

        if (haveFile && havePrincipal) {
            $("#fileSubmit").prop('disabled', false);
        }
    });

    $('#getFiles').click((e) =>
    {
        e.preventDefault();
        getFiles();
    });

    $('#downloadFile').click((e) => {
        e.preventDefault();
        downloadFile();
    });

    function getFiles() {
        let apiPath = BaseApiUrl() + "/api/filemanager/getfilesbyuser";

        return new Promise(function (resolve, reject) {
            $.get({
                url: apiPath
            })
                .done(function (response) {
                    if (response) {
                        setMessage(response);
                    }
                    else {

                    }
                })
                .fail(function (response) {
                    setErrorMessage(response.responseJSON);
                })
        })
    }


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

    function blobToFile(blob, fileName) {
        return new File([blob], fileName, { lastModified: new Date().getTime(), type: blob.type })
    }

    var haveFile = false;
    var havePrincipal = false;
    $("#fileSubmit").prop('disabled', true);
});