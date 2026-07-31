using FluentValidation;
using Identity.Api.Middleware;

using Identity.Application.Users.CreateUsers;
using Identity.Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Identity API",
            Version = "v1"
        });
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
});
builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CreateUsersCommand).Assembly));
builder.Services.AddTransient<IValidator<CreateUsersCommand>, CreateUsersValidator>();
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

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();






