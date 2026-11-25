document.querySelectorAll('.toggle-senha').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var input = document.getElementById(btn.dataset.alvo);
        var icone = btn.querySelector('i');
        var mostrar = input.type === 'password';

        input.type = mostrar ? 'text' : 'password';
        icone.className = mostrar ? 'bi bi-eye-slash' : 'bi bi-eye';
    });
});