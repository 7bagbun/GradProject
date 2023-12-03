const commentApp = Vue.createApp({
    data() {
        return {
            comments: [],
            articles: [],
            isLogin: false,
            myComment: {},
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
                text = Math.ceil(days / 1440) + "天前";
            } else {
                return date;
            }

            return text;
        },
        submitForm(e) {
            console.log("submit");
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
            } else if (Object.keys(this.myComment).length > 1) {
                return "編輯並儲存";
            } else {
                return "發布";
            }
        },
        warningLength() {
            let len = this.textCount;

            if (len > 500) {
                return "text-danger";
            } else {
                return "";
            }
        },
    },
    created() {
        $.get("/comment/getById/" + id, data => {
            this.comments = data;
            Object.assign(this.myComment, data.filter(x => x.IsAuthor)[0]);
        });

        $.get("/article/getById?productId=" + id, data => {
            this.articles = data;
        });

        $.get("/account/isLogin", data => {
            this.isLogin = data.isLogin;
        });
    }
}).mount("#app-comment");