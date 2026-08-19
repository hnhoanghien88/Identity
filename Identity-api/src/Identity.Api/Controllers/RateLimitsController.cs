using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Api.Middleware;
using Identity.Application.Common.Exceptions;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/rate-limiting")]
public sealed class RateLimitsController(
    IdentityDbContext db,
    EndpointDataSource endpointDataSource,
    RateLimitPolicyProvider policyProvider) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "RateLimiting.Read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RateLimitPolicyDto>>>> Get(CancellationToken token)
    {
        var items = await db.RateLimitPolicies.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Priority).ThenBy(x => x.Name)
            .Select(x => ToDto(x)).ToListAsync(token);
        return Ok(new ApiResponse<IReadOnlyList<RateLimitPolicyDto>>(true, items, "Rate limit policies retrieved successfully."));
    }

    [HttpGet("endpoints")]
    [Authorize(Policy = "RateLimiting.Read")]
    public ActionResult<ApiResponse<IReadOnlyList<ApiEndpointDto>>> GetEndpoints()
    {
        var items = endpointDataSource.Endpoints.OfType<RouteEndpoint>()
            .SelectMany(endpoint => (endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? [])
                .Select(method => new ApiEndpointDto(endpoint.RoutePattern.RawText ?? string.Empty, method, endpoint.DisplayName ?? string.Empty)))
            .Where(x => !string.IsNullOrWhiteSpace(x.Route))
            .DistinctBy(x => new { x.Route, x.HttpMethod })
            .OrderBy(x => x.Route).ThenBy(x => x.HttpMethod).ToList();
        return Ok(new ApiResponse<IReadOnlyList<ApiEndpointDto>>(true, items, "API endpoints retrieved successfully."));
    }

    [HttpPost]
    [Authorize(Policy = "RateLimiting.Create")]
    public async Task<ActionResult<ApiResponse<RateLimitPolicyDto>>> Create(SaveRateLimitPolicyRequest request, CancellationToken token)
    {
        Validate(request);
        var value = new RateLimitPolicy();
        Apply(value, request);
        value.CreatedBy = Actor();
        value.CreatedDate = DateTime.UtcNow;
        db.RateLimitPolicies.Add(value);
        await db.SaveChangesAsync(token);
        policyProvider.Invalidate();
        return CreatedAtAction(nameof(Get), new ApiResponse<RateLimitPolicyDto>(true, ToDto(value), "Rate limit policy created successfully."));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "RateLimiting.Update")]
    public async Task<ActionResult<ApiResponse<RateLimitPolicyDto>>> Update(ulong id, SaveRateLimitPolicyRequest request, CancellationToken token)
    {
        Validate(request);
        var value = await db.RateLimitPolicies.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, token)
            ?? throw new NotFoundException($"Rate limit policy '{id}' was not found.");
        if (value.Version != request.Version) return Conflict(new ProblemDetails { Status = 409, Title = "Conflict", Detail = "The policy was changed by another user. Reload and try again." });
        Apply(value, request);
        value.Version++;
        value.UpdatedBy = Actor();
        value.UpdatedDate = DateTime.UtcNow;
        await db.SaveChangesAsync(token);
        policyProvider.Invalidate();
        return Ok(new ApiResponse<RateLimitPolicyDto>(true, ToDto(value), "Rate limit policy updated successfully."));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "RateLimiting.Delete")]
    public async Task<IActionResult> Delete(ulong id, [FromQuery] ulong version, CancellationToken token)
    {
        var value = await db.RateLimitPolicies.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, token);
        if (value is null) return NotFound();
        if (value.Version != version) return Conflict();
        db.RateLimitPolicies.Remove(value);
        await db.SaveChangesAsync(token);
        policyProvider.Invalidate();
        return Ok(new ApiResponse<object>(true, null, "Rate limit policy deleted successfully."));
    }

    private static void Validate(SaveRateLimitPolicyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 150) throw new ArgumentException("Name is required and must not exceed 150 characters.");
        if (string.IsNullOrWhiteSpace(request.RoutePattern) || request.RoutePattern.Length > 300) throw new ArgumentException("Route pattern is required.");
        if (request.PermitLimit == 0 || request.WindowSeconds == 0) throw new ArgumentException("Permit limit and window seconds must be greater than zero.");
        if (request.Algorithm is not ("TokenBucket" or "SlidingWindow" or "FixedWindow")) throw new ArgumentException("Algorithm is invalid.");
    }

    private static void Apply(RateLimitPolicy value, SaveRateLimitPolicyRequest request)
    {
        value.ApplicationId = request.ApplicationId;
        value.Name = request.Name.Trim();
        value.RoutePattern = request.RoutePattern.Trim().TrimStart('/');
        value.HttpMethods = string.IsNullOrWhiteSpace(request.HttpMethods) ? null : request.HttpMethods.Trim().ToUpperInvariant();
        value.PartitionBy = request.PartitionBy.Trim();
        value.Algorithm = request.Algorithm;
        value.PermitLimit = request.PermitLimit;
        value.WindowSeconds = request.WindowSeconds;
        value.BurstLimit = request.BurstLimit;
        value.Priority = request.Priority;
        value.IsActive = request.IsActive;
    }

    private string? Actor() => User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? User.FindFirst("email")?.Value;
    private static RateLimitPolicyDto ToDto(RateLimitPolicy x) => new(x.Id, x.ApplicationId, x.Name, x.RoutePattern, x.HttpMethods, x.PartitionBy, x.Algorithm, x.PermitLimit, x.WindowSeconds, x.BurstLimit, x.Priority, x.IsActive, x.Version);

    public sealed record SaveRateLimitPolicyRequest(ulong? ApplicationId, string Name, string RoutePattern, string? HttpMethods, string PartitionBy, string Algorithm, uint PermitLimit, uint WindowSeconds, uint? BurstLimit, int Priority, bool IsActive, ulong Version = 0);
    public sealed record RateLimitPolicyDto(ulong Id, ulong? ApplicationId, string Name, string RoutePattern, string? HttpMethods, string PartitionBy, string Algorithm, uint PermitLimit, uint WindowSeconds, uint? BurstLimit, int Priority, bool IsActive, ulong Version);
    public sealed record ApiEndpointDto(string Route, string HttpMethod, string DisplayName);
}