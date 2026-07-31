using Identity.Application.Users.Dtos;
using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Users.UpdateUsers;

public sealed class UpdateUsersCommandHandler(
    IUsersRepository repository,
    IValidator<UpdateUsersCommand> validator) : IRequestHandler<UpdateUsersCommand, UsersDto>
{
    public async Task<UsersDto> Handle(UpdateUsersCommand request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var user = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"User '{request.Id}' was not found.");

        if (await repository.CodeExistsAsync(request.Code, request.Id, ct))
        {
            throw new ValidationException($"User code '{request.Code}' already exists.");
        }

        user.Code = request.Code.Trim();
        user.Name = request.Name.Trim();

        await repository.UpdateAsync(user, ct);

        return new UsersDto(
            user.Id,
            user.Code,
            user.Name,
            user.CreatedDate,
            user.IsActive);
    }
}

