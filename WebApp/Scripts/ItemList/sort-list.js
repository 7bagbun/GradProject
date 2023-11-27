Vue.createApp({
    data() {
        return {
            items: [],
            low: "",
            high: ""
        }
    },
    methods: {
        filter() {
            let low = Number(this.low);
            let top = Number(this.high);

            if (low > top && this.high === "") {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].hide = this.items[i].price < low;
                }
            } else if ((top - low) > 0) {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].hide = (this.items[i].price < low) || (this.items[i].price > top);
                }
            } else {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].hide = false;
                }
            }
        },
    },
    computed: {
        filteredList() {
            return this.items.filter(x => !x.hide);
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

