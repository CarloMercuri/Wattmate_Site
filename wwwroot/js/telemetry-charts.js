

function initiateLineChart(_chartElement, _data) {
    let myBarChart = new Chart(_chartElement, {
        type: 'line',
        data: _data,
        options: {
            responsive: true,
            scales: {
                x: {
                    title: {
                        display: true,
                        text: '',
                        font: {
                            padding: 4,
                            size: 20,
                            weight: 'bold',
                            family: 'Arial'
                        },
                        color: 'darkblue'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: '',
                        font: {
                            size: 20,
                            weight: 'bold',
                            family: 'Arial'
                        },
                        color: 'darkblue'
                    },
                    beginAtZero: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Values',
                    }
                }
            }
        }
    });

    return myBarChart;
}

function initiateBarChart(_chartElement, _data) {
    //
    // COST CHART
    //
    console.log(_data)

    let costChartD = new Chart(_chartElement, {
         type: 'bar',
         data: _data,
         options: {
             responsive: true,
             scales: {
                 x: {
                     title: {
                         display: true,
                         text: '',
                         font: {
                             padding: 4,
                             size: 20,
                             weight: 'bold',
                             family: 'Arial'
                         },
                         color: 'darkblue'
                     }
                 },
                 y: {
                     title: {
                         display: true,
                         text: '',
                         font: {
                             size: 20,
                             weight: 'bold',
                             family: 'Arial'
                         },
                         color: 'darkblue'
                     },
                     beginAtZero: true,
                     scaleLabel: {
                         display: true,
                         labelString: 'Values',
                     }
                 }
             }
         }
     });

    return costChartD;
}

