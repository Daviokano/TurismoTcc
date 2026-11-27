const formularioContato = document.getElementById("formularioContato");
const mensagemFormulario = document.getElementById("mensagemFormulario");

if (formularioContato && mensagemFormulario) {
  formularioContato.addEventListener("submit", (evento) => {
    evento.preventDefault();

    mensagemFormulario.textContent =
      "Mensagem enviada com sucesso.";
    mensagemFormulario.classList.add("visivel");
    formularioContato.reset();
  });
}
