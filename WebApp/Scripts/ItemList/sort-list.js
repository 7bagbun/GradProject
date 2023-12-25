Vue.createApp({
    data() {
        return {
            items: [],
            low: "",
            high: "",
            priceOrderDesc: false,
            source: "all"
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
            this.priceOrderDesc = !this.priceOrderDesc;
            if (this.priceOrderDesc) {
                this.items.sort((a, b) => {
                    return b.price - a.price;
                });
            } else {
                this.items.sort((a, b) => {
                    return a.price - b.price;
                });
            }
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
            if (this.source === "all") {
                return this.items.filter(x => this.filterPrice(x.price));
            } else {
                return this.items.filter(x => this.filterPrice(x.price) && x.source === this.source);
            }
        }
    },
    watch: {
        filteredList() {
            this.$nextTick(() => {
                baguetteBox.run(".gallery", {
                    filter: /image\/get\/.*/
                });
            })
        }
    },
    mounted() {
        $.get("/item/get/" + id, data => {
            for (var i = 0; i < data.length; i++) {
                this.items.push({
                    title: data[i].title,
                    source: data[i].source,
                    sourceImage: data[i].sourceImage,
                    price: data[i].price,
                    fprice: data[i].fprice,
                    url: data[i].url,
                    image: data[i].image,
                    hide: false
                });
            }
        });
    }
}).mount("#list-section");
