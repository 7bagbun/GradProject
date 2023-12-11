const app = Vue.createApp({
    data() {
        return {
            list: [],
            type: [],
            selectedType: 1,
            selectedId: 0,
            query: "",
        }
    },
    methods: {
        getProduct() {
            $.get("/adminApi/getProduct", data => {
                this.list = data;
            });
        },
        getProductType() {
            $.get("/item/getTypes", data => {
                this.type = data;
            });
        },
        editProduct(id) {
            this.selectedId = id;
            $("#modalEdit").modal();
        },
        saveChanges() {
            let data = $("#form-edit").serialize();

            $.post("/moderation/editProduct", data, data => {
                this.getProduct();
                swal({
                    text: "編輯成功",
                    icon: "info",
                    buttons: {
                        confirm: "確定"
                    }
                });
            });
        }
    },
    computed: {
        filteredList() {
            let query = this.query.trim();
            let list = this.list.slice();

            if (query !== "") {
                list = list.filter(x =>
                    x.model.indexOf(query) >= 0 ||
                    x.brand.indexOf(query) >= 0 ||
                    x.token?.indexOf(query) >= 0);
            }

            if (this.selectedType === 1) {
                return list;
            } else {
                return list.filter(x => x.type == this.selectedType);
            }
        },
        selectedProduct() {
            let product = this.list.find(x => x.id === this.selectedId);
            return product === undefined ? {} : product;
        },
        editTypeList() {
            return this.type.filter(x => x.id > 1);
        },
    },
    created() {
        this.getProduct();
        this.getProductType();
    }
}).mount("#app");
