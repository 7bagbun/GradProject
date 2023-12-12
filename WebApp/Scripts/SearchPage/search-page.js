$("#query").val(query);

const app = Vue.createApp({
    data() {
        return {
            list: [],
            typeList: [],
            low: "",
            high: "",
            priceOrderDesc: false,
            sortMethod: -1,
            query: "",
            cateId: 1,
            totalPages: 1,
            currentPage: 1,
        }
    },
    methods: {
        filterPrice(price) {
            if (this.low === "" && this.high === "") {
                return true;
            }

            let low = Number(this.low);
            let top = Number(this.high);

            if (low > top && this.high === "") {
                return price > low;
            } else if ((top - low) >= 0) {
                return price >= low && price <= top;
            } else {
                return true;
            }
        },
        sortByPrice() {
            if (this.sortMethod === 0) {
                this.priceOrderDesc = !this.priceOrderDesc;
            }

            if (this.priceOrderDesc) {
                this.list.sort((a, b) => {
                    return a.price - b.price;
                });
            } else {
                this.list.sort((a, b) => {
                    return b.price - a.price;
                });
            }

            this.sortMethod = 0;
        },
        sortByViews() {
            this.list.sort((a, b) => {
                return a.popularity - b.popularity;
            });

            this.sortMethod = 1;
        },
        onInputLow(e) {
            this.low = e.target.value;
        },
        onInputHigh(e) {
            this.high = e.target.value;
        },
        onTypeClick(type) {
            this.cateId = type;
            this.resetCurrentPage();
            this.getItemData();
        },
        onPageNextClick() {
            if (++this.currentPage > this.totalPages) {
                this.currentPage = this.totalPages;
            }

            this.getItemData();
        },
        onPagePrevClick() {
            if (--this.currentPage < 1) {
                this.currentPage = 1;
            }

            this.getItemData();
        },
        getItemData() {
            let params = `?query=${this.query}&cateId=${this.cateId}&page=${this.currentPage}`;
            window.history.replaceState(null, null, params);

            $.get("/item/search" + params, data => {
                this.list = data.products;
                this.totalPages = data.totalPages;
            });
        },
        resetCurrentPage() {
            this.currentPage = 1;
        },
        checkCurrentPage() {
            if (this.currentPage > this.totalPages) {
                this.currentPage = this.totalPages;
            } else if (this.currentPage < 1) {
                this.currentPage = 1;
            }
        }
    },
    computed: {
        filteredList() {
            return this.list.filter(x => this.filterPrice(x.price));
        },
        viewButtonColor() {
            if (this.sortMethod === 1) {
                return "btn-primary";
            } else {
                return "btn-outline-primary";
            }
        },
        priceButtonColor() {
            if (this.sortMethod === 0) {
                return "btn-primary";
            } else {
                return "btn-outline-primary";
            }
        }
    },
    created() {
        this.query = query;
        this.cateId = cateId;

        $.get(`/item/search?query=${this.query}&cateId=${this.cateId}`, data => {
            this.list = data.products;
            this.totalPages = data.totalPages;
        });

        $.get("/item/getTypes", data => {
            this.typeList = data;
        });
    }
}).mount("#app");