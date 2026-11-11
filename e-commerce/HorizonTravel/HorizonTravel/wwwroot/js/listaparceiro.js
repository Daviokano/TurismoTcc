document.addEventListener('DOMContentLoaded', function () {
    var campoBusca = document.getElementById('campoBusca');
    var linhas = document.querySelectorAll('#tabelaParceiros .linha-parceiro');
    var linhaSemResultado = document.getElementById('linhaSemResultado');

    if (!campoBusca) return;

    // tira acentos e deixa minúsculo, pra "açai" achar "Açaí"
    function normalizar(texto) {
        return (texto || '')
            .toString()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .toLowerCase()
            .trim();
    }

    function filtrar() {
        var termo = normalizar(campoBusca.value);
        var termoNumeros = termo.replace(/\D/g, ''); // permite buscar CNPJ com pontos/traço
        var encontrados = 0;

        linhas.forEach(function (linha) {
            var nome = normalizar(linha.querySelector('.nome-parceiro').textContent);
            var cnpj = linha.querySelector('.cnpj-parceiro').textContent.replace(/\D/g, '');

            var combina = termo === '' ||
                nome.indexOf(termo) !== -1 ||
                (termoNumeros !== '' && cnpj.indexOf(termoNumeros) !== -1);

            linha.hidden = !combina;
            if (combina) encontrados++;
        });

        if (linhaSemResultado) {
            linhaSemResultado.hidden = encontrados !== 0;
        }
    }

    campoBusca.addEventListener('input', filtrar);
});
