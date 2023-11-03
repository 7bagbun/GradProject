const formatter = new Intl.NumberFormat("zh-TW");

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
                return `售價: ${formatter.format(params.data.Price)}`
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
                itemStyle: {
                    color: '#dc3545'
                },
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                        {
                            offset: 0,
                            color: '#ff7683'
                        },
                        {
                            offset: 1,
                            color: '#ffc0c6'
                        }
                    ])
                }
            }
        ]
    };

    chart.setOption(option);
}
