
const verticalLines =  [];

const verticalLinePlugin = {
    id: 'verticalLinePlugin',
    afterDraw: (chart) => {
       
        const xScale = chart.scales['x'];
        const ctx = chart.ctx;

        ctx.save();
        verticalLines.forEach(value => {
            const x = xScale.getPixelForValue(value);
            ctx.beginPath();
            ctx.moveTo(x, chart.chartArea.top);
            ctx.lineTo(x, chart.chartArea.bottom);
            ctx.lineWidth = 2;
            ctx.strokeStyle = 'rgba(255, 99, 132, 1)';
            ctx.stroke();
        });
        ctx.restore();
    }
};



function initiateLineChart(_chartElement, _data, clickCallback) {
    console.log("initializing line char")
    Chart.register(verticalLinePlugin);
    let myBarChart = new Chart(_chartElement, {
        type: 'line',
        data: _data,
        options: {
            responsive: true,
            onClick: (event, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    const label = myBarChart.data.labels[index];
                    const value = myBarChart.data.datasets[0].data[index];

                    // Add if not already added
                    if (!verticalLines.includes(label)) {
                        verticalLines.push(label);
                        if (verticalLines.length > 2) {
                            verticalLines.splice(0, 1);
                        }
                        myBarChart.update();
                    }
                    if (clickCallback !== undefined) {
                        // hack
                        let seekIndex = index;
                        let _date = "";
                        while (seekIndex >= 0) {
                            if (seekIndex < 0) break;
                            if (myBarChart.data.labels[seekIndex].length > 8) {
                                _date = myBarChart.data.labels[seekIndex];
                                break;
                            }
                            seekIndex--;
                        }
                        clickCallback(label, value, _date);
                    }
                }
            },
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

