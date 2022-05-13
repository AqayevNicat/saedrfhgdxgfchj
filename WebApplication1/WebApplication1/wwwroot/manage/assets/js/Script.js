$(document).ready(function () {
    $(document).on("click", "#delete", function (e) {
        e.preventDefault();
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                let url = $(this).attr("href");
                fetch(url).then(response => {
                    if (response.ok) {
                        Swal.fire(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        )
                    }

                    return response.text();
                }).then(data => {
                    $(".tableTag").html(data);
                })
            }
        })

    })

    $(document).on("click", "#restore", function (e) {
        e.preventDefault();
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Restore it!'
        }).then((result) => {
            if (result.isConfirmed) {
                let url = $(this).attr("href");
                fetch(url).then(response => {
                    if (response.ok) {
                        Swal.fire(
                            'Restored!',
                            'Your file has been Restored.',
                            'success'
                        )
                    }

                    return response.text();
                }).then(data => {
                    $(".tableTag").html(data);
                })
            }
        })

    })

    $(document).on("click", "#deleteImage", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");
        console.log(url)
        fetch(url).then(response => { return response.text() }).then(data => { $(".updateproduct").html(data) });
    })

    $(document).on("click", "#productDelete", function (e) {
        e.preventDefault();
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                let url = $(this).attr("href");
                fetch(url).then(response => {
                    if (response.ok) {
                        Swal.fire(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        )
                    }

                    return response.text();
                }).then(data => {
                    $(".tableProduct").html(data);
                })
            }
        })

    })

    $(document).on("click", "#productRestore", function (e) {
        e.preventDefault();
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Restore it!'
        }).then((result) => {
            if (result.isConfirmed) {
                let url = $(this).attr("href");
                fetch(url).then(response => {
                    if (response.ok) {
                        Swal.fire(
                            'Restored!',
                            'Your file has been Restored.',
                            'success'
                        )
                    }

                    return response.text();
                }).then(data => {
                    $(".tableProduct").html(data);
                })
            }
        })

    })
})