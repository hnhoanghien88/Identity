using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.UpdateMenu;

public sealed class UpdateMenuCommandHandler(IMenusRepository repository, IValidator<UpdateMenuCommand> validator)
    : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var menu = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{request.Id}' was not found.");
        if (menu.Version != request.Version) throw new ConflictException("The Menu was changed by another user. Reload and try again.");
        if (menu.ApplicationId != request.ApplicationId) throw new ConflictException("A Menu cannot be moved to another Application.", "applicationId");
        if (request.ParentId.HasValue && !await repository.IsParentValidAsync(menu.ApplicationId, request.ParentId.Value, menu.Id, cancellationToken))
            throw new ConflictException("The selected Parent is unavailable or would create a cycle.", "parentId");
        if (request.ResourceId.HasValue && !await repository.IsResourceValidAsync(menu.ApplicationId, request.ResourceId.Value, cancellationToken))
            throw new ConflictException("The selected Resource is unavailable for this Application.", "resourceId");
        var code = MenuRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(menu.ApplicationId, code, menu.Id, cancellationToken))
            throw new ConflictException("Code is already in use for this Application.", "code");
        menu.ParentId = request.ParentId; menu.ResourceId = request.ResourceId; menu.Code = code;
        menu.Name = MenuRules.Clean(request.Name); menu.Route = MenuRules.CleanOptional(request.Route);
        menu.Icon = MenuRules.CleanOptional(request.Icon); menu.SortOrder = request.SortOrder;
        menu.IsVisible = request.IsVisible; menu.IsActive = request.IsActive;
        menu.UpdatedBy = request.Actor; menu.UpdatedDate = DateTime.UtcNow; menu.Version++;
        await repository.SaveAsync(menu, cancellationToken);
        return MenuRules.ToDto(menu);
    }
}
