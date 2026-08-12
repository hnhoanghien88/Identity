using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using ResourceEntity = Identity.Domain.Entities.Resources;

namespace Identity.Application.Tests.Resources;

internal sealed class TestResourcesRepository : IResourcesRepository
{
    public ResourceEntity? Value { get; set; }
    public bool ApplicationAvailable { get; set; } = true;
    public bool Duplicate { get; set; }
    public bool DependenciesExist { get; set; }
    public int SaveCount { get; private set; }

    public Task<ResourceEntity?> GetByIdAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        Task.FromResult(Value is { IsDeleted: false } ? Value : null);

    public Task<bool> IsApplicationAvailableAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        Task.FromResult(ApplicationAvailable);

    public Task<bool> CodeExistsAsync(
        ulong applicationId,
        string code,
        ulong? excludingId,
        CancellationToken cancellationToken) =>
        Task.FromResult(Duplicate);

    public Task<bool> HasDependenciesAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        Task.FromResult(DependenciesExist);

    public Task AddAsync(
        ResourceEntity resource,
        CancellationToken cancellationToken)
    {
        resource.Id = 1;
        resource.Application = Application();
        Value = resource;
        return Task.CompletedTask;
    }

    public Task SaveAsync(
        ResourceEntity resource,
        CancellationToken cancellationToken)
    {
        resource.Application = Application(resource.ApplicationId);
        SaveCount++;
        return Task.CompletedTask;
    }

    public static ResourceEntity Existing(ulong version = 1) =>
        new()
        {
            Id = 1,
            ApplicationId = 1,
            Application = Application(),
            Code = "RESOURCE",
            Name = "Resource",
            ResourceType = "Api",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            Version = version,
        };

    private static Identity.Domain.Entities.Applications Application(ulong id = 1) =>
        new()
        {
            Id = id,
            Code = $"APP{id}",
            Name = $"Application {id}",
            Audience = $"audience-{id}",
            IsActive = true,
            Version = 1,
        };
}