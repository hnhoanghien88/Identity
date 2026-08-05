using MediatR;

namespace Identity.Application.Users.ActivateUsers;

public sealed record ActivateUsersCommand(ulong Id, bool IsActive = true) : IRequest;
