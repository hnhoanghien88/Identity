using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.UpdateUsers;

public sealed record UpdateUsersCommand(ulong Id, string Code, string Name) : IRequest<UsersDto>;


