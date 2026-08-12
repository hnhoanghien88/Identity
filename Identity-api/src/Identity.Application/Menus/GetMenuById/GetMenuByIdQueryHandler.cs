using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.GetMenuById;

public sealed class GetMenuByIdQueryHandler(IMenusReadRepository repository) : IRequestHandler<GetMenuByIdQuery, MenuDto>
{
    public async Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var row = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{request.Id}' was not found.");
        return new(row.Id, row.ApplicationId, row.ApplicationName, row.ParentId, row.ResourceId,
            row.ResourceName, row.Code, row.Name, row.Route, row.Icon, row.SortOrder,
            row.IsVisible, row.IsActive, row.Version, []);
    }
}
