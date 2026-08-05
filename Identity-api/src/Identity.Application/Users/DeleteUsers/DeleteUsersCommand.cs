using MediatR;

namespace Identity.Application.Users.DeleteUsers;

public sealed record DeleteUsersCommand(ulong Id) : IRequest;
