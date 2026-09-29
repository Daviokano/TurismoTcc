document.addEventListener("DOMContentLoaded", function () {
    const track = document.getElementById("depoimentosTrack");
    const dotsContainer = document.getElementById("carrosselDots");
    const slides = Array.from(track.children);

    function getSlidesPerView() {
        if (window.innerWidth <= 600) return 1;
        if (window.innerWidth <= 900) return 2;
        return 3;
    }

    let slidesPerView = getSlidesPerView();
    let totalSlides = Math.ceil(slides.length / slidesPerView);
    let currentSlide = 0;
    let autoplayInterval;

    function updateDots() {
        const dots = dotsContainer.querySelectorAll(".dot");
        dots.forEach((dot, i) => {
            dot.classList.toggle("ativo", i === currentSlide);
        });
    }

    function goToSlide(index) {
        currentSlide = (index + totalSlides) % totalSlides;
        const slideWidth = slides[0].getBoundingClientRect().width;
        const gap = 20;
        const offset = currentSlide * slidesPerView * (slideWidth + gap);
        track.style.transform = `translateX(-${offset}px)`;
        updateDots();
    }

    function nextSlide() {
        goToSlide(currentSlide + 1);
    }

    function startAutoplay() {
        autoplayInterval = setInterval(nextSlide, 4000);
    }

    function resetAutoplay() {
        clearInterval(autoplayInterval);
        startAutoplay();
    }

    dotsContainer.querySelectorAll(".dot").forEach((dot, i) => {
        dot.addEventListener("click", () => {
            goToSlide(i);
            resetAutoplay();
        });
    });

    window.addEventListener("resize", function () {
        const newSlidesPerView = getSlidesPerView();
        if (newSlidesPerView !== slidesPerView) {
            slidesPerView = newSlidesPerView;
            totalSlides = Math.ceil(slides.length / slidesPerView);
            currentSlide = 0;
            goToSlide(0);
        }
    });

    goToSlide(0);
    startAutoplay();

    track.parentElement.addEventListener("mouseenter", () => clearInterval(autoplayInterval));
    track.parentElement.addEventListener("mouseleave", startAutoplay);
});