$(document).ready(function () {
    let apiPath = BaseApiUrl() + "/api/filemanager/savefiles";

    function BaseApiUrl() {
        return window.location.origin;
    }
    
    $('#fileSubmit').click((e) => {
        let myFile = $('#fileTest').prop('files');
        const fd = new FormData();
        
        // add all selected files
        for (var i = 0; i < myFile.length; i++) {
            fd.append("test", myFile[i], myFile[i].name);
            console.log(myFile[i].name);
        }
        console.log(fd.getAll('test'));

        fetch(apiPath, { method: "POST", body: fd }).then((response) => {
            if (response.ok) {
                return response.json();
            }
            throw new Error('Something went wrong');
        })
            .then((responseJson) => {
                // Do something with the response
            })
            .catch((error) => {
                console.log(error)
            });
        e.preventDefault();
    });    
});