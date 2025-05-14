document.addEventListener('DOMContentLoaded', function() {
    // Get elements
    
    const bottomButton = document.querySelector('.enter-site-link');
    const hiddenTexts = document.querySelectorAll('.hidden-text');
    
    // Create a timeline
    const tl = gsap.timeline();
    
    // Add animations to the timeline - this will play automatically
    tl.to(hiddenTexts, {
        y: "0%",
        opacity: 1,
        duration: 1.5,
        stagger: 0.25,
        ease: "power4.out"
    })
    .to(bottomButton, {
        opacity: 1,
        duration: 0.8,
        ease: "power2.out"
    }, "+=0.5"); // Add a small delay before showing the button
    
    
});