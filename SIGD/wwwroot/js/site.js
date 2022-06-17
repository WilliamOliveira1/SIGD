$(document).ready(function () {    
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