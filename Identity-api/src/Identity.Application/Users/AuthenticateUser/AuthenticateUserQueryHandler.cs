using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.AuthenticateUser;

public sealed class AuthenticateUserQueryHandler(
    IUsersRepository repository,
    IPasswordHasher passwordHasher) : IRequestHandler<AuthenticateUserQuery, UsersDto>
{
    public async Task<UsersDto> Handle(AuthenticateUserQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrEmpty(request.Password))
            throw new UnauthorizedAccessException("Invalid code or password.");

        var user = await repository.GetByCodeAsync(request.Code, ct);
        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid code or password.");

        return new UsersDto(user.Id, user.Code, user.Name, user.CreatedDate, user.IsActive);
    }
}
