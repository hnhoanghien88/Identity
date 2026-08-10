using Identity.Application.Users.Dtos;
using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using MediatR;
using Identity.Application.Common.Exceptions;
using UsersEntity = Identity.Domain.Entities.Users;

namespace Identity.Application.Users.CreateUsers;

public sealed class CreateUsersCommandHandler(
    IUsersRepository repository,
    IPasswordHasher passwordHasher,
    IValidator<CreateUsersCommand> validator) : IRequestHandler<CreateUsersCommand, UsersDto>
{
    public async Task<UsersDto> Handle(CreateUsersCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        if (await repository.CodeExistsAsync(request.Code, null, cancellationToken))
        {
            throw new ConflictException("Code is already in use.", "code");
        }

        if (await repository.EmailExistsAsync(request.Email, null, cancellationToken))
        {
            throw new ConflictException("Email is already in use.", "email");
        }

        var user = new UsersEntity
        {
            Code = request.Code,
            Email = request.Email.Trim(),
            Name = request.Name.Trim(),
            CreatedDate = DateTime.UtcNow,
            Password = passwordHasher.Hash(request.Password),
            IsActive = true
        };

        await repository.AddAsync(user, cancellationToken);

        return new UsersDto(
            user.Id,
            user.Code,
            user.Email,
            user.Name,
            user.CreatedDate,
            user.IsActive,
            user.Version);
    }
}

