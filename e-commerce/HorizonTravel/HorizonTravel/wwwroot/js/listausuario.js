document.addEventListener('DOMContentLoaded', function () {
    const campoBusca = document.getElementById('campoBusca');
    const linhas = document.querySelectorAll('#tabelaUsuarios .linha-usuario');
    const linhaSemResultado = document.getElementById('linhaSemResultado');

    if (!campoBusca) return;

    // remove acentos e deixa minúsculo
    const normalizar = (texto) =>
        (texto || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().trim();

    // deixa só números (para buscar CPF com ou sem pontuação)
    const soNumeros = (texto) => (texto || '').replace(/\D/g, '');

    campoBusca.addEventListener('input', function () {
        const termo = normalizar(campoBusca.value);
        const termoNumeros = soNumeros(campoBusca.value);
        let visiveis = 0;

        linhas.forEach(function (linha) {
            const nome = normalizar(linha.querySelector('.nome-usuario')?.textContent);
            const cpf = soNumeros(linha.querySelector('.cpf-usuario')?.textContent);

            const achouNome = nome.includes(termo);
            const achouCpf = termoNumeros.length > 0 && cpf.includes(termoNumeros);
            const mostrar = termo === '' || achouNome || achouCpf;

            linha.hidden = !mostrar;
            linha.style.display = mostrar ? '' : 'none';
            if (mostrar) visiveis++;
        });

        linhaSemResultado.hidden = visiveis > 0;
    });
});

(function () {
    const REGISTROS_POR_PAGINA = 10;

    const campoBusca = document.getElementById('campoBusca');
    const botaoBuscar = document.getElementById('botaoBuscar');
    let paginacao = document.getElementById('paginacao');
    const linhaSemResultado = document.getElementById('linhaSemResultado');
    const linhas = Array.from(document.querySelectorAll('.linha-funcionario, .linha-usuario'));

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

            const nome = normalizar(linha.querySelector('.nome-funcionario, .nome-usuario').textContent);
            const cpf = linha.querySelector('.cpf-funcionario, .cpf-usuario').textContent;

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