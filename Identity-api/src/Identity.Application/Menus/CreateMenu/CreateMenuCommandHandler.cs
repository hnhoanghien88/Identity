using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Menus.Dtos;
using MediatR;
using MenuEntity = Identity.Domain.Entities.Menus;

namespace Identity.Application.Menus.CreateMenu;

public sealed class CreateMenuCommandHandler(IMenusRepository repository, IValidator<CreateMenuCommand> validator)
    : IRequestHandler<CreateMenuCommand, MenuDto>
{
    public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        await ValidateRelations(request.ApplicationId, request.ParentId, request.ResourceId, null, cancellationToken);
        var code = MenuRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(request.ApplicationId, code, null, cancellationToken))
            throw new ConflictException("Code is already in use for this Application.", "code");
        var menu = new MenuEntity { ApplicationId = request.ApplicationId, ParentId = request.ParentId,
            ResourceId = request.ResourceId, Code = code, Name = MenuRules.Clean(request.Name),
            Route = MenuRules.CleanOptional(request.Route), Icon = MenuRules.CleanOptional(request.Icon),
            SortOrder = request.SortOrder, IsVisible = request.IsVisible, IsActive = request.IsActive,
            IsDeleted = false, Version = 1, CreatedBy = request.Actor, CreatedDate = DateTime.UtcNow };
        await repository.AddAsync(menu, cancellationToken);
        return MenuRules.ToDto(menu);
    }

    private async Task ValidateRelations(ulong applicationId, ulong? parentId, ulong? resourceId, ulong? movingId, CancellationToken token)
    {
        if (!await repository.IsApplicationAvailableAsync(applicationId, token))
            throw new ConflictException("The selected Application is unavailable.", "applicationId");
        if (parentId.HasValue && !await repository.IsParentValidAsync(applicationId, parentId.Value, movingId, token))
            throw new ConflictException("The selected Parent is unavailable or would create a cycle.", "parentId");
        if (resourceId.HasValue && !await repository.IsResourceValidAsync(applicationId, resourceId.Value, token))
            throw new ConflictException("The selected Resource is unavailable for this Application.", "resourceId");
    }
}
