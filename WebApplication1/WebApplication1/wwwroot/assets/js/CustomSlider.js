$(document).ready(function () {
	// prodct details slider active
	$('.product-large-slider').slick({
		fade: true,
		arrows: false,
		asNavFor: '.pro-nav'
	});


	// product details slider nav active
	$('.pro-nav').slick({
		slidesToShow: 4,
		asNavFor: '.product-large-slider',
		arrows: false,
		focusOnSelect: true
	});

	// testimonial carousel active js
	$('.testimonial-active').slick({
		dots: true,
		arrows: false,
		responsive: [
			{
				breakpoint: 992,
				settings: {
					dots: false
				}
			}
		]
	});


	// image zoom effect
	$('.img-zoom').zoom();
})