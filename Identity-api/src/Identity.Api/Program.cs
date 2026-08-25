using FluentValidation;
using Identity.Api.Middleware;
using Identity.Api.Authorization;
using Identity.Application.Actions.CreateAction;
using Identity.Application.Actions.UpdateAction;
using Identity.Application.Roles.CreateRole;
using Identity.Application.Roles.UpdateRole;
using Identity.Application.Applications.CreateApplication;
using Identity.Application.Applications.UpdateApplication;
using Identity.Application.Resources.CreateResource;
using Identity.Application.Resources.UpdateResource;
using Identity.Application.Menus.CreateMenu;
using Identity.Application.Menus.UpdateMenu;
using System.Text;
using Identity.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.Tokens;

using Identity.Application.Users.CreateUsers;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Behaviors;
using Identity.Infrastructure;
using Microsoft.OpenApi;
using StackExchange.Redis;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

var performanceLogPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "logs",
    "performance-.log");
var applicationLogPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "logs",
    "application-.log");
builder.Host.UseSerilog((context, _, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext();

    if (context.HostingEnvironment.IsDevelopment())
        loggerConfiguration.WriteTo.Console();
    else
        loggerConfiguration.WriteTo.Console(new JsonFormatter(renderMessage: true));

    loggerConfiguration
        .WriteTo.File(
            applicationLogPath,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 25 * 1024 * 1024,
            retainedFileCountLimit: 30,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1),
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] CorrelationId={CorrelationId} TraceId={TraceId} UserId={UserId} StatusCode={StatusCode} ElapsedMs={ElapsedMs} | {Message:lj}{NewLine}{Exception}")
        .WriteTo.Logger(performanceLogger => performanceLogger
        .Filter.ByIncludingOnly(logEvent =>
            logEvent.Level >= LogEventLevel.Warning
            && logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
            && sourceContext.ToString().Contains(
                "PerformanceBehavior",
                StringComparison.Ordinal))
        .WriteTo.File(
            performanceLogPath,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 10 * 1024 * 1024,
            retainedFileCountLimit: 14,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1),
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
});
var performance = builder.Configuration.GetSection(PerformanceOptions.SectionName)
    .Get<PerformanceOptions>() ?? new PerformanceOptions();
if (performance.SlowRequestThresholdMilliseconds <= 0)
    throw new InvalidOperationException("Observability:SlowRequestThresholdMilliseconds must be greater than zero.");
builder.Services.AddSingleton(performance);


builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthorizationCache, AuthorizationCache>();
builder.Services.AddScoped<RateLimitPolicyProvider>();
var rateLimiting = builder.Configuration.GetSection(RateLimitingOptions.SectionName)
    .Get<RateLimitingOptions>() ?? new RateLimitingOptions();
if (rateLimiting.Store is not ("Redis" or "InMemory"))
    throw new InvalidOperationException("RateLimiting:Store must be either 'Redis' or 'InMemory'.");
if (rateLimiting.PolicyCacheSeconds <= 0)
    throw new InvalidOperationException("RateLimiting:PolicyCacheSeconds must be greater than zero.");
if (rateLimiting.FailureMode is not ("Open" or "Closed"))
    throw new InvalidOperationException("RateLimiting:FailureMode must be either 'Open' or 'Closed'.");
builder.Services.Configure<RateLimitingOptions>(
    builder.Configuration.GetSection(RateLimitingOptions.SectionName));
if (rateLimiting.Store == "Redis")
{
    var redisConnection = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("ConnectionStrings:Redis is required when RateLimiting:Store is Redis.");
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
    builder.Services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();
}
else
{
    builder.Services.AddSingleton<IRateLimitStore, InMemoryRateLimitStore>();
}
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
    throw new InvalidOperationException("Jwt:Key must contain at least 32 bytes.");
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var externalAuthentication = builder.Configuration
    .GetSection(ExternalAuthenticationOptions.SectionName)
    .Get<ExternalAuthenticationOptions>() ?? new ExternalAuthenticationOptions();
builder.Services.Configure<ExternalAuthenticationOptions>(
    builder.Configuration.GetSection(ExternalAuthenticationOptions.SectionName));
if (externalAuthentication.Google.Enabled)
{
    if (string.IsNullOrWhiteSpace(externalAuthentication.Google.ClientId)
        || string.IsNullOrWhiteSpace(externalAuthentication.Google.ClientSecret))
        throw new InvalidOperationException("Google external authentication credentials are missing.");
    if (!Uri.TryCreate(externalAuthentication.FrontendLoginUrl, UriKind.Absolute, out var frontendLoginUri)
        || (frontendLoginUri.Scheme != Uri.UriSchemeHttps && !frontendLoginUri.IsLoopback))
        throw new InvalidOperationException("ExternalAuthentication:FrontendLoginUrl must use HTTPS except on loopback.");
}
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
var authenticationBuilder = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
authenticationBuilder.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        // Audience is validated against the active application record below.
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
    {
        if (context.Principal?.FindFirst("token_type")?.Value != "access")
            context.Fail("Only access tokens are accepted.");
        else
        {
            var applicationIdValue = context.Principal.FindFirst("application_id")?.Value;
            var applicationCode = context.Principal.FindFirst("application_code")?.Value;
            var audience = context.Principal.FindFirst("aud")?.Value;
            if (!ulong.TryParse(applicationIdValue, out var applicationId))
            {
                context.Fail("The access token application is invalid.");
                return;
            }

            var application = await context.HttpContext.RequestServices
                .GetRequiredService<IApplicationsReadRepository>()
                .GetByIdAsync(applicationId, context.HttpContext.RequestAborted);
            if (application is null
                || !application.IsActive
                || !string.Equals(application.Code, applicationCode, StringComparison.Ordinal)
                || !string.Equals(application.Audience, audience, StringComparison.Ordinal))
            {
                context.Fail("The access token audience is invalid.");
                return;
            }

            var subject = context.Principal.FindFirst("sub")?.Value;
            if (!ulong.TryParse(subject, out var userId)) context.Fail("The access token subject is invalid.");
            else
            {
                var user = await context.HttpContext.RequestServices.GetRequiredService<IUsersRepository>()
                    .GetByIdAsync(userId, context.HttpContext.RequestAborted);
                if (user is null || !user.IsActive) context.Fail("The user account is unavailable.");
                else if (!int.TryParse(context.Principal.FindFirst("permissionversion")?.Value, out var permissionVersion)
                    || permissionVersion != user.PermissionVersion)
                    context.Fail("The authorization version is stale.");
            }
        }
    },
        OnChallenge = async context =>
        {
            context.HandleResponse();
            await ProblemDetailsAuthorizationResults.WriteUnauthorizedAsync(context.HttpContext);
        },
        OnForbidden = context =>
            ProblemDetailsAuthorizationResults.WriteForbiddenAsync(context.HttpContext),
    };
});
authenticationBuilder.AddCookie("ExternalCookie", options =>
{
    options.Cookie.Name = "__Host-identity-external";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Path = "/";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});
if (externalAuthentication.Google.Enabled)
{
    authenticationBuilder.AddGoogle("Google", options =>
    {
        options.SignInScheme = "ExternalCookie";
        options.ClientId = externalAuthentication.Google.ClientId;
        options.ClientSecret = externalAuthentication.Google.ClientSecret;
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.ClaimActions.MapJsonKey("email_verified", "verified_email");
        options.ClaimActions.MapJsonKey("email_verified", "email_verified");
        options.ClaimActions.MapJsonKey("picture", "picture");
    });
}
builder.Services.AddAuthorization(options =>
{
    options.AddPermissionPolicies();
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Identity API",
            Version = "v1"
        });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter the access token returned by POST /login."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
    });
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
});
builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssembly(typeof(CreateUsersCommand).Assembly);
    c.AddOpenBehavior(typeof(PerformanceBehavior<,>));
    c.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddTransient<IValidator<CreateUsersCommand>, CreateUsersValidator>();
builder.Services.AddTransient<IValidator<CreateApplicationCommand>, CreateApplicationValidator>();
builder.Services.AddTransient<IValidator<CreateActionCommand>, CreateActionValidator>();
builder.Services.AddTransient<IValidator<UpdateActionCommand>, UpdateActionValidator>();
builder.Services.AddTransient<IValidator<CreateRoleCommand>, CreateRoleValidator>();
builder.Services.AddTransient<IValidator<UpdateRoleCommand>, UpdateRoleValidator>();
builder.Services.AddTransient<IValidator<UpdateApplicationCommand>, UpdateApplicationValidator>();
builder.Services.AddTransient<IValidator<CreateResourceCommand>, CreateResourceValidator>();
builder.Services.AddTransient<IValidator<UpdateResourceCommand>, UpdateResourceValidator>();
builder.Services.AddTransient<IValidator<CreateMenuCommand>, CreateMenuValidator>();
builder.Services.AddTransient<IValidator<UpdateMenuCommand>, UpdateMenuValidator>();
builder.Services.AddTransient<
    IValidator<Identity.Application.Users.UpdateUsers.UpdateUsersCommand>,
    Identity.Application.Users.UpdateUsers.UpdateUsersValidator>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<StructuredRequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<DynamicRateLimitMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
