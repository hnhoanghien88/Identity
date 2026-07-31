using Identity.Application.Users.Dtos;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Users.GetUsersById;

public sealed class GetUsersByIdQueryHandler(IUsersReadRepository repository)
    : IRequestHandler<GetUsersByIdQuery, UsersDto>
{
    public async Task<UsersDto> Handle(GetUsersByIdQuery request, CancellationToken ct)
    {
        return await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"User '{request.Id}' was not found.");
    }
}

