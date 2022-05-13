$(document).ready(function () {
    $(document).on("click", ".addToBasket", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");
        fetch(url).then(res => {
            return res.text()
            console.log(url)
        }).then(data => {
            $(".minicart-inner").html(data);
            fetch("home/count/").then(res => {
                return res.text()
            }).then(data => {
                $(".notification").html(data)
            })
        })
    })
    $(document).on("click", "#addbasketbtn", function (e) {
        e.preventDefault()

        let url = $("#basketform").attr("action")
        let count = $("#productcount").val();

        var e = document.getElementById("colorids");
        var colorID = e.options[e.selectedIndex].value;

        var e = document.getElementById("sizeids");
        var sizeID = e.options[e.selectedIndex].value;

        console.log(colorID)
        console.log(sizeID)

        url = url + "?count=" + count + "&colorid=" + colorID + "&sizeid=" + sizeID;
        fetch(url).then(response => {
            return response.text();
        }).then(data => {
            $(".minicart-inner").html(data)
            fetch("home/count/").then(res => {
                return res.text()
            }).then(data => {
                $(".notification").html(data)
            })
        })
    })

    $(".productModal").click(function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => response.text())
            .then(data => {
                $(".modal-content").html(data)

                $('.product-large-slider').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    dots: false,
                    fade: true,
                    asNavFor: '.quick-view-thumb',
                    speed: 400,
                });

                $('.pro-nav').slick({
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    asNavFor: '.quick-view-image',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true,
                    speed: 400,
                });

                $("#productQuickModal").modal("show")
            })
    })
    $(document).on("click", ".qtybtn", function (e) {
        e.preventDefault()
        let url = $(this).attr("href");
        var count = parseInt($(this).parent().find('input').val());
        if (isNaN(count)) {
            count = 1;
        }

        var id = $(this).parent().find('input').attr("data-id");

        let color = parseFloat($(this).parent().parent().parent().find(".prColor").text());
        let size = parseFloat($(this).parent().parent().parent().find(".prSize").text());
        if ($(this).hasClass("dec")) {

            if (count != 0) {
                count--;
            }
        }
        else {
            count++;
        }

        fetch("Basket/Update" + "?id=" + id + "&count=" + count + "&color=" + color + "&size=" + size).then(response => {

            fetch("Basket/GetBasket").then(response => response.text())
                .then(data => $(".minicart-inner").html(data))
            return response.text()

        }).then(data => $(".basketContainer").html(data))

        $(this).parent().find('input').val(parseFloat(count));
    })

    $(document).on("click", ".deletecard", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => {
            fetch("Basket/GetBasket").then(response => response.text()).then(data => $(".header-cart").html(data))


            fetch("home/count").then(res => {
                return res.text()
            }).then(data => {
                $(".notification").html(data)
            })
            return response.text()
        }).then(data => {
            $(".basketContainer").html(data);
        })

    })

    $(document).on("click", ".deletebasket", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => {
            fetch("Basket/GetCard").then(response => response.text()).then(data => $(".basketContainer").html(data))

            fetch("home/count").then(res => {
                return res.text()
            }).then(data => {
                $(".notification").html(data)
            })
            return response.text()
        }).then(data => $(".minicart-inner").html(data))
    })

    $(document).on("keyup", "#searchBtn", function (e) {
        e.preventDefault()
        console.log($(this).val())
        let url = $("#searchForm").attr("action");
        fetch(url + "?key=" + $(this).val()).then(res => {
            return res.text()
        }).then(data => {
            $("#productList").html(data)
        })
    })
})