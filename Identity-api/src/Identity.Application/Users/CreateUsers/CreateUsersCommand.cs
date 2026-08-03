using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.CreateUsers;

public sealed record CreateUsersCommand(string Code, string Name, string Password) : IRequest<UsersDto>;

