namespace Identity.Application.Applications.Dtos;

public sealed record PagedApplicationsDto(IReadOnlyList<ApplicationDto> Items, int TotalCount, int Page, int PageSize);
