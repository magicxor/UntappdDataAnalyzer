function renderAsColumnChart(chartName, data) {
    Highcharts.chart(chartName, {
        chart: {
            type: 'column'
        },
        title: {
            text: chartName
        },
        xAxis: {
            type: 'category'
        },
        plotOptions: {
            column: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        series: [{
            name: chartName,
            data: data
        }]
    });
}

function renderAsPieChart(chartName, data) {
    Highcharts.chart(chartName, {
        chart: {
            type: 'pie'
        },
        title: {
            text: chartName
        },
        plotOptions: {
            pie: {
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                }
            }
        },
        series: [{
            name: chartName,
            data: data
        }]
    });
}

function renderRatingAndCountColumnChart(chartName1, chartName2, data1, data2) {
    Highcharts.chart(chartName1, {
        chart: {
            type: 'column'
        },
        title: {
            text: chartName1 + " and " + chartName2
        },
        xAxis: {
            type: 'category'
        },
        yAxis: [{ // Primary yAxis
            title: {
                text: 'Rating'
            },
            max: 5,
            showLastLabel: false
        }, { // Secondary yAxis
            title: {
                text: 'Count'
            },
            opposite: true,
            visible: false
        }],
        plotOptions: {
            column: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        series: [{
            name: chartName1,
            data: data1
        },
        {
            name: chartName2,
            data: data2,
            yAxis: 1
        }]
    });
}

function renderValueAndCountColumnChart(chartName1, chartName2, data1, data2) {
    Highcharts.chart(chartName1, {
        chart: {
            type: 'column'
        },
        title: {
            text: chartName1 + " and " + chartName2
        },
        xAxis: {
            type: 'category'
        },
        yAxis: [{ // Primary yAxis
            showLastLabel: false
        }, { // Secondary yAxis
            title: {
                text: 'Count'
            },
            opposite: true,
            visible: false
        }],
        plotOptions: {
            column: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        series: [{
                name: chartName1,
                data: data1
            },
            {
                name: chartName2,
                data: data2,
                yAxis: 1
            }]
    });
}