$.ajax({
    type: "GET",
    url: `/pricehistory/get?productId=${id}`,
    async: true,
    success: (data) => {
        chartInit(data);
    },
    failure: (err) => {
        console.log(err);
    }
})

function chartInit(data) {
    const chart = echarts.init(document.getElementById('main'));

    option = {
        title: {
            text: '歷史最低價格',
            left: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: (params) => {
                return `售價: ${params.data.Price}`
            }
        },
        dataset: {
            source: data
        },
        xAxis: {
            type: 'category',
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                name: 'Step Start',
                type: 'line',
                step: 'end',
            }
        ]
    };

    chart.setOption(option);
}
