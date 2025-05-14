document.addEventListener('DOMContentLoaded', function () {
    // Initialize the carousel with proper options
    
    let myCarouselElement = document.getElementById('myCarousel');
    if (myCarouselElement) {
        var myCarousel = new bootstrap.Carousel(myCarouselElement, {
            interval: 3000,  // Adjust auto-play speed if needed
            wrap: true,
            keyboard: true,
            
        });
    
    myCarousel.next(); // Go to next slide
    myCarousel.prev(); // Go to previous slide
    myCarousel.pause(); // Pause the carousel
    myCarousel.cycle(); // Resume the carousel
    }

   
});