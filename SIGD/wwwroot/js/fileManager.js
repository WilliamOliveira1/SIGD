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
            if (e.nextSibling?.innerText) {
                $emails.push(e.nextSibling.innerText.replace(/ /g, ''));
                console.log($emails);
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
                    return data;
                }).catch((err) => {
                    console.log(err);
                })
            })
            .then((responseJson) => {
                let message = "";
                let status = false;
                if (responseJson === true) {
                    message = "Todos arquivos foram salvos.";
                    status = true;
                }
                else if (responseJson === false) {
                    message = "Devido a um erro nenhum arquivo foi salvo, por favor tente novamente!";
                }
                else {
                    message = responseJson
                }                
                setMessageWithStatus(message, status);
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
        let numberAdd = 0;
        if (value === "0") {
            $("#principalsList option").each(function () {
                if (this.value === "0") {
                    $(`#principalsList option[value=${this.value}]`).remove();
                }
                else
                {
                    $(`#principalsList option[value=${this.value}]`).remove();
                    $("#listSelected").append(`
                            <div id="principalSelect${numberAdd}">
                                <span id="${this.value}" class="principalEmail">${this.innerHTML}</span>
                                <i id="delPrincipal${numberAdd}" class="fa-regular fa-circle-xmark pointer removePrincipalSelect" title="remover"></i>
                            </div>`
                    );
                    havePrincipal = true;
                    numberAdd++;
                }
            });
        }
        else {
            $(`#principalsList option[value=${value}]`).remove();
            $("#listSelected").append(`
                            <div id="principalSelect${numberAdd}">
                                <span id="${value}" class="principalEmail">${email}</span>
                                <i id="delPrincipal${numberAdd}" class="fa-regular fa-circle-xmark pointer removePrincipalSelect" title="remover"></i>
                            </div>`
            );
            havePrincipal = true;
            numberAdd++;
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

        $(".removePrincipalSelect").click((e) => {
            let test = e.delegateTarget.id
            let test1 = $(`#${test}`).parent();
            let id = test1[0].childNodes[1].id
            let email = test1[0].childNodes[1].innerHTML
            let listCount = $('#principalsList option').length;
            if (listCount === 0) {
                $("#schoolsAdd").attr("hidden", true);
                $('#principalsList').append($('<option></option>').val(id).text(email));
            }
            else {
                $('#principalsList').append($('<option></option>').val(id).text(email));
            }
            
            $("#schoolsAdd").attr("hidden", false);
            $(`#${test1[0].id}`).remove();
        });

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

    function getFiles() {
        let apiPath = BaseApiUrl() + "/api/filemanager/getfilesbyuser";

        return new Promise(function (resolve, reject) {
            $.get({
                url: apiPath
            })
                .done(function (response) {
                    if (response) {
                        setMessageWithStatus(response);
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