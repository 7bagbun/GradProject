const formatter = new Intl.NumberFormat("zh-TW");

$.ajax({
    type: "GET",
    url: "/pricehistory/get?productId=" + id,
    async: true,
    success: (data) => {
        chartInit(data);
    },
    failure: (err) => {
        console.log(err);
    }
})

function chartInit(data) {
    const chart = echarts.init(document.getElementById("main"));

    option = {
        title: {
            text: "歷史最低價格",
            left: "center"
        },
        tooltip: {
            trigger: "item",
            formatter: (params) => {
                return `售價: NT$${formatter.format(params.data.Price)}<br>日期: ${params.data.UpdatedTime}`
            }
        },
        dataset: {
            source: data
        },
        xAxis: {
            type: "category",
        },
        yAxis: {
            type: "value",
            minInterval: 100,
        },
        series: [
            {
                name: "Step Start",
                type: "line",
                step: "end",
                itemStyle: {
                    color: "#88c0d0"
                },
                areaStyle: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                        {
                            offset: 0,
                            color: "#88c0d0"
                        },
                        {
                            offset: 1,
                            color: "#eee"
                        }
                    ])
                }
            }
        ]
    };

    chart.setOption(option);
}
