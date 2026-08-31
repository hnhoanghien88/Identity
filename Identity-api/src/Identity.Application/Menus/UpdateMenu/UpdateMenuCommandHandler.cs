using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.UpdateMenu;

public sealed class UpdateMenuCommandHandler(IMenusRepository repository)
    : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{request.Id}' was not found.");
        if (menu.Version != request.Version) throw new ConflictException("The Menu was changed by another user. Reload and try again.");
        var applicationChanged = menu.ApplicationId != request.ApplicationId;
        if (applicationChanged
            && !await repository.IsApplicationAvailableAsync(request.ApplicationId, cancellationToken))
            throw new ConflictException("The selected Application is unavailable.", "applicationId");
        if (applicationChanged && await repository.HasChildrenAsync(menu.Id, cancellationToken))
            throw new ConflictException(
                "A Menu with children cannot be moved to another Application.",
                "applicationId");
        if (request.ParentId.HasValue
            && !await repository.IsParentValidAsync(
                request.ApplicationId,
                request.ParentId.Value,
                menu.Id,
                cancellationToken))
            throw new ConflictException("The selected Parent is unavailable or would create a cycle.", "parentId");
        if (request.ResourceId.HasValue
            && !await repository.IsResourceValidAsync(
                request.ApplicationId,
                request.ResourceId.Value,
                cancellationToken))
            throw new ConflictException("The selected Resource is unavailable for this Application.", "resourceId");
        var code = MenuRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(request.ApplicationId, code, menu.Id, cancellationToken))
            throw new ConflictException("Code is already in use for this Application.", "code");
        menu.ApplicationId = request.ApplicationId;
        menu.ParentId = request.ParentId; menu.ResourceId = request.ResourceId; menu.Code = code;
        menu.Name = MenuRules.Clean(request.Name); menu.Route = MenuRules.CleanOptional(request.Route);
        menu.Icon = MenuRules.CleanOptional(request.Icon); menu.SortOrder = request.SortOrder;
        menu.IsVisible = request.IsVisible; menu.IsActive = request.IsActive;
        menu.UpdatedBy = request.Actor; menu.UpdatedDate = DateTime.UtcNow; menu.Version++;
        await repository.SaveAsync(menu, cancellationToken);
        return MenuRules.ToDto(menu);
    }
}
