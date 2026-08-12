namespace Identity.Application.Resources.Dtos;

public sealed record PagedResourcesDto(
    IReadOnlyList<ResourceDto> Items,
    int TotalCount,
    int Page,
    int PageSize);