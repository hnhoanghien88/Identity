using Identity.Application.Users.Dtos;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Users.UpdateUsers;

public sealed class UpdateUsersCommandHandler(
    IUsersRepository repository) : IRequestHandler<UpdateUsersCommand, UsersDto>
{
    public async Task<UsersDto> Handle(UpdateUsersCommand request, CancellationToken ct)
    {

        var user = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"User '{request.Id}' was not found.");

        if (user.Version != request.Version)
            throw new ConflictException();

        if (await repository.CodeExistsAsync(request.Code, request.Id, ct))
        {
            throw new ConflictException("Code is already in use.", "code");
        }

        if (await repository.EmailExistsAsync(request.Email, request.Id, ct))
        {
            throw new ConflictException("Email is already in use.", "email");
        }

        user.Code = request.Code;
        user.Email = request.Email.Trim();
        user.Name = request.Name.Trim();
        user.UpdatedDate = DateTime.UtcNow;
        user.Version++;

        await repository.UpdateAsync(user, ct);

        return new UsersDto(
            user.Id,
            user.Code,
            user.Email,
            user.Name,
            user.CreatedDate,
            user.IsActive,
            user.PermissionVersion,
            user.Version);
    }
}

