document.addEventListener("DOMContentLoaded", function () {
    const campoBusca = document.getElementById("campoBusca");
    const linhas = document.querySelectorAll("#tabelaUsuarios tbody .linha-usuario");
    const linhaSemResultado = document.getElementById("linhaSemResultado");

    function normalizar(texto) {
        return texto
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }

    function somenteDigitos(texto) {
        return texto.replace(/\D/g, "");
    }

    campoBusca.addEventListener("input", function () {
        const termo = normalizar(campoBusca.value.trim());
        const termoDigitos = somenteDigitos(termo);
        let encontrados = 0;

        linhas.forEach(function (linha) {
            const nome = normalizar(linha.querySelector(".nome-usuario").textContent);
            const cpf = somenteDigitos(linha.querySelector(".cpf-usuario").textContent);

            const achouNome = nome.includes(termo);
            const achouCpf = termoDigitos !== "" && cpf.includes(termoDigitos);
            const visivel = termo === "" || achouNome || achouCpf;

            linha.hidden = !visivel;
            if (visivel) encontrados++;
        });

        linhaSemResultado.hidden = encontrados > 0;
    });
});