namespace Copasogo.Application.Generic;

public static class ApplicationMessages
{
    public static class Generic
    {
        public const string RecursoNaoEncontrado = "Recurso nao encontrado.";
        public const string UsuarioNaoAutenticado = "Usuario nao autenticado.";
    }

    public static class Auth
    {
        public const string CredenciaisInvalidas = "E-mail ou senha invalidos.";
        public const string ContaBloqueada = "Conta bloqueada temporariamente por tentativas invalidas.";
        public const string EmailObrigatorio = "E-mail e obrigatorio.";
        public const string SenhaObrigatoria = "Senha e obrigatoria.";
    }
}
