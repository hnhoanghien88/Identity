using Identity.Application.Users.Dtos;
using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using MediatR;
using UsersEntity = Identity.Domain.Entities.Users;

namespace Identity.Application.Users.CreateUsers;

public sealed class CreateUsersCommandHandler(
    IUsersRepository repository,
    IValidator<CreateUsersCommand> validator) : IRequestHandler<CreateUsersCommand, UsersDto>
{
    public async Task<UsersDto> Handle(CreateUsersCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        if (await repository.CodeExistsAsync(request.Code, null, cancellationToken))
        {
            throw new ValidationException($"User code '{request.Code}' already exists.");
        }

        var user = new UsersEntity
        {
            Id = Guid.NewGuid(),
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await repository.AddAsync(user, cancellationToken);

        return new UsersDto(user.Id, user.Code, user.Name, user.CreatedDate, user.IsActive);
    }
}

