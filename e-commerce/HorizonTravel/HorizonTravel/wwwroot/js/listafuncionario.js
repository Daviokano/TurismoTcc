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


(function () {
    const REGISTROS_POR_PAGINA = 3;

    const campoBusca = document.getElementById('campoBusca');
    const botaoBuscar = document.getElementById('botaoBuscar');
    let paginacao = document.getElementById('paginacao');
    const linhaSemResultado = document.getElementById('linhaSemResultado');
    const linhas = Array.from(document.querySelectorAll('.linha-funcionario'));

    // Se a view não tiver a div da paginação, cria uma logo abaixo da tabela
    if (!paginacao) {
        paginacao = document.createElement('div');
        paginacao.id = 'paginacao';
        paginacao.className = 'paginacao';
        const cartao = document.querySelector('.cartao-tabela');
        if (cartao) cartao.insertAdjacentElement('afterend', paginacao);
    }

    console.log('[lista funcionarios] linhas encontradas:', linhas.length);

    let linhasFiltradas = linhas.slice();
    let paginaAtual = 1;

    function normalizar(texto) {
        return (texto || '')
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '');
    }

    function somenteDigitos(texto) {
        return (texto || '').replace(/\D/g, '');
    }

    function filtrar() {
        const termo = normalizar(campoBusca ? campoBusca.value.trim() : '');
        const termoDigitos = somenteDigitos(termo);

        linhasFiltradas = linhas.filter(function (linha) {
            if (!termo) return true;

            const nome = normalizar(linha.querySelector('.nome-funcionario').textContent);
            const cpf = linha.querySelector('.cpf-funcionario').textContent;

            const achouNome = nome.includes(termo);
            const achouCpf = termoDigitos.length > 0 &&
                somenteDigitos(cpf).includes(termoDigitos);

            return achouNome || achouCpf;
        });

        paginaAtual = 1;
        renderizar();
    }

    function renderizar() {
        const totalPaginas = Math.max(1, Math.ceil(linhasFiltradas.length / REGISTROS_POR_PAGINA));
        if (paginaAtual > totalPaginas) paginaAtual = totalPaginas;

        const inicio = (paginaAtual - 1) * REGISTROS_POR_PAGINA;
        const fim = inicio + REGISTROS_POR_PAGINA;

        linhas.forEach(function (linha) {
            linha.hidden = true;
        });

        linhasFiltradas.slice(inicio, fim).forEach(function (linha) {
            linha.hidden = false;
        });

        linhaSemResultado.hidden = linhasFiltradas.length > 0;

        renderizarPaginacao(totalPaginas);
    }

    function criarBotao(conteudo, pagina, opcoes) {
        opcoes = opcoes || {};
        const botao = document.createElement('button');
        botao.type = 'button';
        botao.className = 'botao-pagina' + (opcoes.atual ? ' pagina-atual' : '');
        botao.innerHTML = conteudo;
        botao.disabled = !!opcoes.desabilitado;

        if (opcoes.rotulo) botao.setAttribute('aria-label', opcoes.rotulo);

        botao.addEventListener('click', function () {
            paginaAtual = pagina;
            renderizar();
        });

        return botao;
    }

    function renderizarPaginacao(totalPaginas) {
        paginacao.innerHTML = '';

        paginacao.appendChild(criarBotao('&lsaquo;', paginaAtual - 1, {
            desabilitado: paginaAtual === 1,
            rotulo: 'Página anterior'
        }));

        for (let i = 1; i <= totalPaginas; i++) {
            paginacao.appendChild(criarBotao(i, i, { atual: i === paginaAtual }));
        }

        paginacao.appendChild(criarBotao('&rsaquo;', paginaAtual + 1, {
            desabilitado: paginaAtual === totalPaginas,
            rotulo: 'Próxima página'
        }));
    }

    if (botaoBuscar) botaoBuscar.addEventListener('click', filtrar);

    if (campoBusca) {
        campoBusca.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                filtrar();
            }
        });

        // Filtra enquanto digita; se preferir só no botão/Enter, apague este bloco
        campoBusca.addEventListener('input', filtrar);
    }

    renderizar();
})();
