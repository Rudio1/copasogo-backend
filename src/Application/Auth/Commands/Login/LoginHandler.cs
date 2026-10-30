using Copasogo.Application.Auth.Services.Interfaces;
using Copasogo.Application.Generic;
using FluentValidation;

namespace Copasogo.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Senha);

public sealed record LoginUsuarioResponse(Guid Id, string Nome, string Email);

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, LoginUsuarioResponse Usuario);

public interface ILoginHandler
{
    Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default);
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress().WithMessage(ApplicationMessages.Auth.EmailObrigatorio);
        RuleFor(command => command.Senha).NotEmpty().WithMessage(ApplicationMessages.Auth.SenhaObrigatoria);
    }
}

public sealed class LoginHandler(IUserAccountService contas, IJwtTokenService tokens, IValidator<LoginCommand> validator) : ILoginHandler
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var credenciais = await contas.ValidarAsync(command.Email, command.Senha, cancellationToken);
        if (!credenciais.Success || credenciais.User is null)
        {
            var mensagem = credenciais.Falha == FalhaCredencial.Bloqueada
                ? ApplicationMessages.Auth.ContaBloqueada
                : ApplicationMessages.Auth.CredenciaisInvalidas;
            return Result<LoginResponse>.Fail(mensagem);
        }

        var roles = await contas.ObterRolesAsync(credenciais.User.Id, cancellationToken);
        var access = tokens.GenerateAccessToken(credenciais.User, roles);
        return Result<LoginResponse>.Ok(new LoginResponse(
            access.AccessToken,
            access.ExpiresAt,
            new LoginUsuarioResponse(credenciais.User.Id, credenciais.User.Nome, credenciais.User.Email)));
    }
}
