function getChartData() {
    let apiPath = BaseApiUrl() + "/api/filemanager/getDataChart";

    return new Promise(function (resolve, reject) {
        $.get({
            url: apiPath,
            contentType: 'application/json'
        })
            .done(function (response) {
                if (response) {
                    let obj = JSON.parse(response)
                    setSupervisorChart(obj);//Create method to load table with users
                }
            })
            .fail(function (response) {
                setErrorMessage(response.responseJSON);
            })
    })
}

function setSupervisorChart(data) {
    let yValuesNotRead = [];
    let yValuesRead = [];
    let xValues = [];
    let yValues = [];
    let fontColor = [];
    let barColors = ["red", "green", "blue", "orange", "brown"];
    data.forEach(function (e, i) {
        xValues.push(e.Item1);
        yValues.push(e.Item2.length);
        yValuesNotRead.push(e.Item2.filter(x => x.Status == false).length);
        yValuesRead.push(e.Item2.filter(x => x.Status == true).length);
    });
    let quantyValuesRead = yValuesRead.filter(x => x > 0).length;
    let quantyValues = yValues.filter(x => x > 0).length;
    let quantyValuesNotRead = yValuesNotRead.filter(x => x > 0).length;


    for (let i = 0; i <= yValues.length; i++) {
        fontColor.push("white");
    }

    if (quantyValues > 0) {
        $("#noTotalFilesMessage").remove();
        $("#totalChart").show();
        new Chart("totalChart", {
            type: "pie",
            data: {
                labels: xValues,
                datasets: [{
                    backgroundColor: barColors,
                    data: yValues
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    labels: {
                        render: 'value',
                        fontColor: fontColor,
                    },
                    title: {
                        display: true,
                        text: 'Total de arquivos por diretor.'
                    }
                }
            }
        });
    }
    else {
        $("#totalChart").hide();
        $("#noTotalFilesMessage").remove();
        $("#filesReaded").append(`<h6 id="noTotalFilesMessage">Sem arquivos nesta seção</h6>`);
    }

    if (quantyValuesNotRead > 0) {
        $("#noFileNotReadedMessage").remove();
        $("#totalNotReadChart").show();
        new Chart("totalNotReadChart", {
            type: "pie",
            data: {
                labels: xValues,
                datasets: [{
                    backgroundColor: barColors,
                    data: yValuesNotRead
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    labels: {
                        render: 'value',
                        fontColor: fontColor,
                    },
                    title: {
                        display: true,
                        text: 'Total de arquivos por diretor.'
                    }
                }
            }
        });
    }
    else {
        $("#totalNotReadChart").hide();
        $("#noFileNotReadedMessage").remove();
        $("#filesReaded").append(`<h6 id="noFileNotReadedMessage">Sem arquivos nesta seção</h6>`);
    }
        
    if (quantyValuesRead > 0) {
        $("#noFileReadedMessage").remove();
        $("#totalReadChart").show();
        new Chart("totalReadChart", {
            type: "pie",
            data: {
                labels: xValues,
                datasets: [{
                    backgroundColor: barColors,
                    data: yValuesRead
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    labels: {
                        render: 'value',
                        fontColor: fontColor,
                    },
                    title: {
                        display: true,
                        text: 'Total de arquivos por diretor.'
                    }
                }
            }
        });
    }
    else {
        $("#totalReadChart").hide();
        $("#noFileReadedMessage").remove();
        $("#filesReaded").append(`<h6 id="noFileReadedMessage">Sem arquivos nesta seção</h6>`);
    }    
}

$(document).ready(function () {
    if (window.location.href.indexOf("MainBoard") > -1) {
        getChartData();
    }
});

search = (key, inputArray) => {
    for (let i = 0; i < inputArray.length; i++) {
        if (inputArray[i].name === key) {
            return inputArray[i];
        }
    }
}