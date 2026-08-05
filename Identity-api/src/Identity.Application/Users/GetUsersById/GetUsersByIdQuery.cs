using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.GetUsersById;

public sealed record GetUsersByIdQuery(ulong Id) : IRequest<UsersDto>;


