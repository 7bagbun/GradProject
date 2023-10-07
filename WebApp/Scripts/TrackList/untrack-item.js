function untrack(pid) {
    $.ajax({
        type: 'POST',
        url: '/trackitem/trackajax',
        async: true,
        data: {
            productId: pid
        },
        success: (data) => {
            if (data.isSucceed) {
                window.location = "/trackitem/tracklist";
            } else {
                window.location = data.redirUrl;
            }
        },
        failure: (err) => {
            console.log(err);
        }
    })
}