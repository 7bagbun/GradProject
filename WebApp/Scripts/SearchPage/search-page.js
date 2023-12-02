const app = Vue.createApp({
    data() {
        return {
            list: [],
            low: "",
            high: "",
            priceOrderDesc: false,
            sortMethod: -1,
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
                price > low;
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
                    return a.price < b.price;
                });
            } else {
                this.list.sort((a, b) => {
                    return a.price > b.price;
                });
            }

            this.sortMethod = 0;
        },
        sortByViews() {
            this.list.sort((a, b) => {
                return a.popularity > b.popularity;
            });

            this.sortMethod = 1;
        },
        onInputLow(e) {
            this.low = e.target.value;
        },
        onInputHigh(e) {
            this.high = e.target.value;
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
    mounted() {
        $.get("/item/search?query=" + query, data => {
            this.list = data;
            console.log(this.list);
        });
    }
}).mount("#app");