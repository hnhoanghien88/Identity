using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Menus.DeleteMenu;

public sealed class DeleteMenuCommandHandler(IMenusRepository repository) : IRequestHandler<DeleteMenuCommand>
{
    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{request.Id}' was not found.");
        if (menu.Version != request.Version) throw new ConflictException("The Menu was changed by another user. Reload and try again.");
        if (await repository.HasChildrenAsync(menu.Id, cancellationToken))
            throw new ConflictException("This Menu cannot be deleted while child Menus exist.");
        menu.IsDeleted = true; menu.IsActive = false; menu.UpdatedBy = request.Actor;
        menu.UpdatedDate = DateTime.UtcNow; menu.Version++;
        await repository.SaveAsync(menu, cancellationToken);
    }
}
