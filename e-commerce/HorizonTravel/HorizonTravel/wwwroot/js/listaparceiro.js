// Busca + paginação das listas (funcionários e usuários).
// Funciona com as linhas .linha-funcionario / .linha-usuario / .linha-parceiro.
// Para mudar a quantidade por página, use data-por-pagina na div da paginação:
//   <div class="paginacao" id="paginacao" data-por-pagina="10"></div>
(function () {
    const campoBusca = document.getElementById('campoBusca');
    const botaoBuscar = document.getElementById('botaoBuscar');
    const paginacao = document.getElementById('paginacao');
    const linhaSemResultado = document.getElementById('linhaSemResultado');
    const linhas = Array.from(document.querySelectorAll('.linha-funcionario, .linha-usuario, .linha-parceiro'));

    if (!paginacao) return;

    const REGISTROS_POR_PAGINA = Number(paginacao.dataset.porPagina) || 10;

    let linhasFiltradas = linhas.slice();
    let paginaAtual = 1;

    // tira acentos e deixa minúsculo, pra "jose" achar "José"
    function normalizar(texto) {
        return (texto || '')
            .toString()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .toLowerCase()
            .trim();
    }

    function somenteDigitos(texto) {
        return (texto || '').replace(/\D/g, '');
    }

    function filtrar() {
        const termo = normalizar(campoBusca ? campoBusca.value : '');
        const termoDigitos = somenteDigitos(termo);

        linhasFiltradas = linhas.filter(function (linha) {
            if (!termo) return true;

            const elNome = linha.querySelector('.nome-funcionario, .nome-usuario, .nome-parceiro');
            const elCpf = linha.querySelector('.cpf-funcionario, .cpf-usuario, .cnpj-parceiro');

            const nome = normalizar(elNome ? elNome.textContent : '');
            const cpf = somenteDigitos(elCpf ? elCpf.textContent : '');

            const achouNome = nome.includes(termo);
            const achouCpf = termoDigitos.length > 0 && cpf.includes(termoDigitos);

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

        if (linhaSemResultado) {
            linhaSemResultado.hidden = linhasFiltradas.length > 0;
        }

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

        // filtra enquanto digita
        campoBusca.addEventListener('input', filtrar);
    }

    renderizar();
})();


// Confere se "Senha" e "Confirmar senha" são iguais antes de enviar o cadastro.
// Serve para o formulário de funcionário e para o de usuário.
(function () {
    const formularios = [
        { form: 'formCadastroFuncionario', senha: 'senhaFun' },
        { form: 'formCadastroUsuario', senha: 'senhaUsu' }
    ];

    formularios.forEach(function (item) {
        const form = document.getElementById(item.form);
        if (!form) return; // esse formulário não está nesta página

        const senha = document.getElementById(item.senha);
        const confirmar = document.getElementById('confirmarSenha');
        const erro = document.getElementById('erroSenha');

        if (!senha || !confirmar || !erro) return;

        function senhasIguais() {
            return senha.value === confirmar.value;
        }

        confirmar.addEventListener('input', function () {
            erro.hidden = senhasIguais() || confirmar.value === '';
        });

        form.addEventListener('submit', function (e) {
            if (!senhasIguais()) {
                e.preventDefault();
                erro.hidden = false;
                confirmar.focus();
            }
        });

        form.addEventListener('reset', function () {
            erro.hidden = true;
        });
    });
})();
