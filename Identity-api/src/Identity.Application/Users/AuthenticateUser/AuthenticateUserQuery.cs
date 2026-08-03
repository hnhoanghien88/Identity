using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.AuthenticateUser;

public sealed record AuthenticateUserQuery(string Code, string Password) : IRequest<UsersDto>;
