function getChartDataPrincipal() {
    let apiPath = BaseApiUrl() + "/api/filemanager/getDataChartPrincipal";

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
    let xValues = [];
    let yValues = [];
    let fontColor = [];
    let barColors = ["red", "green", "blue", "orange", "brown"];
    data.forEach(function (e, i) {
        xValues.push(e.Item1);
        yValues.push(e.Item2.length);
    });
    let quantyValues = yValues.filter(x => x > 0).length;

    for (let i = 0; i <= yValues.length; i++) {
        fontColor.push("white");
    }

    if (quantyValues > 0) {
        $("#noTotalFilesMessage").remove();
        $("#totalChart").show();
        new Chart("totalChartPrincipal", {
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
}

$(document).ready(function () {
    if (window.location.href.indexOf("MainBoard") > -1) {
        getChartDataPrincipal();
    }
});

search = (key, inputArray) => {
    for (let i = 0; i < inputArray.length; i++) {
        if (inputArray[i].name === key) {
            return inputArray[i];
        }
    }
}