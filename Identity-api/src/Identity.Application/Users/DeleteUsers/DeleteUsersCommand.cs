using MediatR;

namespace Identity.Application.Users.DeleteUsers;

public sealed record DeleteUsersCommand(Guid Id) : IRequest;
