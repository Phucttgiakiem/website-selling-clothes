const multipleItemCarousel = document.querySelector('#carouselEx');
console.log(multipleItemCarousel)
if (window.matchMedia("(min-width:576px)").matches) {
    const carousel = new boostrap.carousel(multipleItemCarousel, {
        interval: false
    })
    var carouselWidth = $('#carouselExample .carousel-inner')[0].scrollWidth;
    var cardWidth = $('#carouselExample .carousel-item').width();
    var scrollPosition = 0;
    $('#carouselExample .carousel-control-next').on('click', function () {
        if (scrollPosition < (carouselWidth - (cardWidth * 4))) {
            console.log('next');
            scrollPosition = scrollPosition + cardWidth;
            $('#carouselExample .carousel-inner').animate({ scrollLeft: scrollPosition }, 600);

        }
    });
    $('#carouselExample .carousel-control-prev').on('click', function () {
        if (scrollPosition > 0) {
            console.log('prev');
            scrollPosition = scrollPosition - cardWidth;
            $('#carouselExample .carousel-inner').animate({ scrollLeft: scrollPosition }, 600);

        }
    })
} else {
    $(multipleItemCarousel).addClass('slide');
}