using Identity.Application.Users.CreateUsers;
using Identity.Application.Users.UpdateUsers;

namespace Identity.Application.Tests.Users;

public sealed class UserValidatorsTests
{
    [Theory]
    [InlineData("has space")]
    [InlineData("người-dùng")]
    [InlineData("user@email")]
    [InlineData("")]
    public void Create_rejects_invalid_code(string code)
    {
        var result = new CreateUsersValidator().Validate(
            new CreateUsersCommand(
                code,
                "user@example.com",
                "Name",
                "password1"));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateUsersCommand.Code));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public void Create_rejects_invalid_email(string email)
    {
        var result = new CreateUsersValidator().Validate(
            new CreateUsersCommand(
                "user-1",
                email,
                "Name",
                "password1"));

        Assert.Contains(
            result.Errors,
            x => x.PropertyName == nameof(CreateUsersCommand.Email));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("")]
    public void Create_rejects_invalid_password(string password)
    {
        var result = new CreateUsersValidator().Validate(
            new CreateUsersCommand(
                "user-1",
                "user@example.com",
                "Name",
                password));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateUsersCommand.Password));
    }

    [Fact]
    public void Update_requires_a_positive_version()
    {
        var result = new UpdateUsersValidator().Validate(
            new UpdateUsersCommand(
                1,
                "user-1",
                "user@example.com",
                "Name",
                0));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateUsersCommand.Version));
    }
}
