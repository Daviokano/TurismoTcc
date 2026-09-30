document.addEventListener("DOMContentLoaded", function () {
    const trilho = document.getElementById("depoimentosTrack");
    const containerPontos = document.getElementById("carrosselDots");
    const slides = Array.from(trilho.children);

    function obterSlidesPorVisao() {
        if (window.innerWidth <= 600) return 1;
        if (window.innerWidth <= 900) return 2;
        return 3;
    }

    let slidesPorVisao = obterSlidesPorVisao();
    let totalSlides = Math.ceil(slides.length / slidesPorVisao);
    let slideAtual = 0;
    let intervaloAutoplay;

    trilho.style.transition = "transform 0.3s ease";

    function atualizarPontos() {
        const pontos = containerPontos.querySelectorAll(".dot");
        pontos.forEach((ponto, i) => {
            ponto.classList.toggle("ativo", i === slideAtual);
        });
    }

    function irParaSlide(indice) {
        slideAtual = (indice + totalSlides) % totalSlides;
        const larguraSlide = slides[0].getBoundingClientRect().width;
        const espaco = 20;
        const deslocamento = slideAtual * slidesPorVisao * (larguraSlide + espaco);
        trilho.style.transform = `translateX(-${deslocamento}px)`;
        atualizarPontos();
    }

    function proximoSlide() {
        irParaSlide(slideAtual + 1);
    }

    function iniciarAutoplay() {
        intervaloAutoplay = setInterval(proximoSlide, 2500);
    }

    function reiniciarAutoplay() {
        clearInterval(intervaloAutoplay);
        iniciarAutoplay();
    }

    containerPontos.querySelectorAll(".dot").forEach((ponto, i) => {
        ponto.addEventListener("click", () => {
            irParaSlide(i);
            reiniciarAutoplay();
        });
    });

    window.addEventListener("resize", function () {
        const novoSlidesPorVisao = obterSlidesPorVisao();
        if (novoSlidesPorVisao !== slidesPorVisao) {
            slidesPorVisao = novoSlidesPorVisao;
            totalSlides = Math.ceil(slides.length / slidesPorVisao);
            slideAtual = 0;
            irParaSlide(0);
        }
    });

    irParaSlide(0);
    iniciarAutoplay();

    trilho.parentElement.addEventListener("mouseenter", () => clearInterval(intervaloAutoplay));
    trilho.parentElement.addEventListener("mouseleave", iniciarAutoplay);
});