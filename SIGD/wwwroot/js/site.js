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
                        let data = response;

                        data.forEach(function (e, i) {
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
        var conceptName = $('#principalsList').find(":selected");
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
        getFiles();
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

    var haveFile = false;
    var havePrincipal = false;
    $("#fileSubmit").prop('disabled', true);
});