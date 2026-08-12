using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Applications.GetApplicationById;

public sealed class GetApplicationByIdQueryHandler(IApplicationsReadRepository repository) : IRequestHandler<GetApplicationByIdQuery, ApplicationDto>
{
    public async Task<ApplicationDto> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken) => await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"Application '{request.Id}' was not found.");
}
