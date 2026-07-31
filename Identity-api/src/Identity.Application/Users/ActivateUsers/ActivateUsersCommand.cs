using MediatR;

namespace Identity.Application.Users.ActivateUsers;

public sealed record ActivateUsersCommand(Guid Id, bool IsActive = true) : IRequest;
