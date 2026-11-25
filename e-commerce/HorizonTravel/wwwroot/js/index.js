document.addEventListener("DOMContentLoaded", function () {
    const trilho = document.getElementById("depoimentosTrack");
    const containerPontos = document.getElementById("carrosselDots");
    const slides = Array.from(trilho.children);
    
    // Como colocamos 3 dots no HTML, o total de páginas é 3.
    // (Se você adicionar mais dots no HTML, mude esse número)
    let totalPaginas = 3; 
    let slidesPorVisao = obterSlidesPorVisao();
    let paginaAtual = 0;
    let intervaloAutoplay;

    function obterSlidesPorVisao() {
        if (window.innerWidth <= 600) return 1;
        if (window.innerWidth <= 900) return 2;
        return 3;
    }

    // Função para atualizar as bolinhas
    function atualizarPontos() {
        const pontos = containerPontos.querySelectorAll(".dot");
        pontos.forEach((ponto, i) => {
            // Remove o ativo de todos
            ponto.classList.remove("ativo");
            ponto.style.backgroundColor = "rgba(255,255,255,0.4)"; // Cor inativa
            
            // Adiciona no atual
            if (i === paginaAtual) {
                ponto.classList.add("ativo");
                ponto.style.backgroundColor = "#fff"; // Cor ativa
            }
        });
    }

    function irParaPagina(indice) {
        if (indice >= totalPaginas) {
            paginaAtual = 0;
        } else if (indice < 0) {
            paginaAtual = totalPaginas - 1;
        } else {
            paginaAtual = indice;
        }

        let deslocamento = 0;
        if (paginaAtual > 0) {
            const larguraSlide = slides[0].getBoundingClientRect().width;
            const gap = 20; 
            deslocamento = paginaAtual * slidesPorVisao * (larguraSlide + gap);
        }

        trilho.style.transform = `translateX(-${deslocamento}px)`;
        atualizarPontos();
    }

    function proximaPagina() {
        irParaPagina(paginaAtual + 1);
    }

    function iniciarAutoplay() {
        clearInterval(intervaloAutoplay);
        intervaloAutoplay = setInterval(proximaPagina, 4000);
    }

    function reiniciarAutoplay() {
        clearInterval(intervaloAutoplay);
        iniciarAutoplay();
    }

    // Configura os cliques nas bolinhas
    const pontos = containerPontos.querySelectorAll(".dot");
    pontos.forEach((ponto, i) => {
        ponto.addEventListener("click", () => {
            console.log("Clicou no dot:", i); // Teste no console
            irParaPagina(i);
            reiniciarAutoplay();
        });
    });

    // Inicializa
    iniciarAutoplay();

    // Pausa ao passar o mouse
    trilho.parentElement.addEventListener("mouseenter", () => clearInterval(intervaloAutoplay));
    trilho.parentElement.addEventListener("mouseleave", iniciarAutoplay);
});