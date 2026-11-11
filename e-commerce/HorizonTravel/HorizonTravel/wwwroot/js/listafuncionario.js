document.addEventListener('DOMContentLoaded', function () {
    var campoBusca = document.getElementById('campoBusca');
    var linhas = document.querySelectorAll('#tabelaFuncionarios .linha-funcionario');
    var linhaSemResultado = document.getElementById('linhaSemResultado');

    if (!campoBusca) return;

    // tira acentos e deixa minúsculo, pra "jose" achar "José"
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
        var termoNumeros = termo.replace(/\D/g, ''); // permite buscar CPF com pontos/traço
        var encontrados = 0;

        linhas.forEach(function (linha) {
            var nome = normalizar(linha.querySelector('.nome-funcionario').textContent);
            var cpf = linha.querySelector('.cpf-funcionario').textContent.replace(/\D/g, '');

            var combina = termo === '' ||
                nome.indexOf(termo) !== -1 ||
                (termoNumeros !== '' && cpf.indexOf(termoNumeros) !== -1);

            linha.hidden = !combina;
            if (combina) encontrados++;
        });

        if (linhaSemResultado) {
            linhaSemResultado.hidden = encontrados !== 0;
        }
    }

    campoBusca.addEventListener('input', filtrar);

    // roda uma vez ao carregar (mostra a mensagem se a lista vier vazia)
    filtrar();
});