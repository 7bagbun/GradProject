const commentApp = Vue.createApp({
    data() {
        return {
            comments: [],
            articles: [],
            isLogin: false,
            hasWrote: false,
            myComment: {},
            reportComment: {},
            report: { temp: "0", reason: "" }
        }
    },
    methods: {
        getStars(rating) {
            let html = "";

            for (let i = 1; i <= 5; i++) {
                if (i <= rating) {
                    html += '<span class="fa fa-star colored-star"></span>';
                } else {
                    html += '<span class="fa fa-star uncolored-star"></span>';
                }
            }

            return html;
        },
        getDaysFromToday(date) {
            let min = (new Date().getTime() - new Date(date).getTime()) / 60000;
            let text = "";

            if (min < 60) {
                text = Math.ceil(min) + "分前";
            } else if (min < 1440) {
                text = Math.ceil(min / 60) + "小時前";
            } else if (min < 10080) {
                text = Math.ceil(min / 1440) + "天前";
            } else {
                return date.split(" ")[0];
            }

            return text;
        },
        getComments() {
            $.get("/comment/getById/" + id, data => {
                this.comments = data;
                Object.assign(this.myComment, data.find(x => x.IsAuthor));
                this.hasWrote = Object.keys(this.myComment).length > 1;
            });

            $.get("/article/getById?productId=" + id, data => {
                this.articles = data;
            });
        },
        selectReport(commentId) {
            if (!this.isLogin) {
                location.href = `/account/loginPage?referer=/item/list/${id}?tab=comment`;
                return;
            }

            this.reportComment = this.comments.find(x => x.Id === commentId);
            $("#report-modal").modal();
        },
        submitComment(e) {
            e.preventDefault();
            const data = $("#comment-form").serialize();
            let url = this.hasWrote ? "edit" : "create";

            $.post("/comment/" + url, data, () => {
                this.getComments();
            });
        },
        submitReport(e) {
            e.preventDefault();

            const data = $("#form-report").serialize();
            $.post("/comment/report", data, () => {
                $("#report-modal").modal("hide");

                swal({
                    text: "檢舉成功",
                    icon: "info",
                    buttons: {
                        confirm: "確定"
                    },
                });
            });
        },
        deleteComment(commentId) {
            swal({
                text: "你確定要刪除評論嗎?",
                icon: "warning",
                buttons: {
                    cancel: "取消",
                    confirm: "確定"
                },
                dangerMode: true
            }).then((decision) => {
                if (decision) {
                    $.post("/comment/delete", { commentId: commentId }, (data) => {
                        if (data.isSucceed) {
                            this.getComments();
                            this.myComment = {};
                        }
                    });
                }
            });
        }
    },
    computed: {
        textCount() {
            if (this.myComment.Content !== undefined) {
                return this.myComment.Content.length;
            } else {
                return 0;
            }
        },
        saveButtonText() {
            if (!this.isLogin) {
                return "登入後即可發布評論";
            } else if (this.hasWrote) {
                return "編輯並儲存";
            } else {
                return "發布";
            }
        },
        warningLength() {
            let len = this.textCount;

            if (len === 0 || len > 500) {
                return "text-danger";
            } else {
                return "";
            }
        },
        getReportComment() {
            if (this.reportComment !== {}) {
                return this.reportComment;
            } else {
                return { Id: 0, Content: "", Author: "" };
            }
        },
        validateMyComment() {
            if (this.myComment.Content === undefined) {
                return false;
            }

            let len = this.myComment.Content.length;
            let rating = this.myComment.Rating;

            if (len < 1 || len > 500 || rating === undefined) {
                return false;
            } else {
                return true;
            }
        },
        validateReport() {
            if (this.report.temp === "0") {
                return false;
            } else if (this.report.temp === "其他" && this.report.reason === "") {
                return false;
            } else if (this.report.reason.length > 500) {
                return false;
            } else {
                return true;
            }
        }
    },
    created() {
        this.getComments();

        $.get("/account/isLogin", data => {
            this.isLogin = data.isLogin;
        });
    }
}).mount("#app-comment");