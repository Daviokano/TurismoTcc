// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('.CPF').mask('000.000.000-00');
    $('.CEP').mask('0000-000', { reverse: true });
    $('.Dinheiro').mask('000.000.000.000.000,00', { reverse: true });
    $('.Telefone').mask('(00) 00000-0000');
    $('.DataNasc').mask('00/00/0000');
});