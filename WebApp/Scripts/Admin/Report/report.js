const app = Vue.createApp({
    data() {
        return {
            list: [],
            selected: 0,
            filter: "p"
        }
    },
    methods: {
        getReport() {
            $.get("/adminApi/reportComment", data => {
                this.list = data;
            });
        },
        status(status) {
            switch (status) {
                case "p":
                    return `<i class="far fa-clock mr-2 text-secondary"> 待處理</i>`;
                    break;
                case "a":
                    return `<i class="far fa-check-circle mr-2 true"> 已移除</i>`;
                    break;
                case "r":
                    return `<i class="far fa-times-circle mr-2 false"> 已否決</i>`;
                    break;
                default:
            }
        },
        resolve(decision) {
            let report = this.selectedReport;

            if (decision) {
                $.post("/moderation/resolveReport", { commentId: report.commentId, approve: true });
            } else {
                $.post("/moderation/resolveReport", { commentId: report.commentId, approve: false });
            }

            this.getReport();
        }
    },
    computed: {
        selectedReport() {
            if (this.list.length === 0) {
                return {};
            }

            return this.list[this.selected];
        },
        filteredReport() {
            if (this.filter === "e") {
                return this.list;
            }

            return this.list.filter(x => x.status === this.filter);
        }
    },
    created() {
        this.getReport();
    }
}).mount("#app");