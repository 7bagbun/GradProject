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
        error: (err) => {
            console.log(err);
        }
    })
}