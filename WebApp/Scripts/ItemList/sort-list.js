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
        filterPrice() {
            let low = Number(this.low);
            let top = Number(this.high);

            if (low > top && this.high === "") {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].hide = this.items[i].price < low;
                }
            } else if ((top - low) >= 0) {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].hide = (this.items[i].price < low) || (this.items[i].price > top);
                }
            } else {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].hide = false;
                }
            }
        },
        sortByPrice() {
            this.priceOrderDesc = !this.priceOrderDesc;
            if (this.priceOrderDesc) {
                this.items.sort((a, b) => {
                    return a.price < b.price;
                });
            } else {
                this.items.sort((a, b) => {
                    return a.price > b.price;
                });
            }
        }
    },
    computed: {
        filteredList() {
            if (this.source === "all") {
                return this.items.filter(x => !x.hide);
            } else {
                return this.items.filter(x => !x.hide && x.source === this.source);
            }
        }
    },
    watch: {
        filteredList() {
            this.$nextTick(() => {
                let a = baguetteBox.run(".gallery", {
                    filter: /image\/get\/.*/
                });

                console.log(a);
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

