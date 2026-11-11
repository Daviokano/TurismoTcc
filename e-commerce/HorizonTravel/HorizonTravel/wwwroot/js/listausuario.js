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