using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.AuthenticateUser;
using UserEntity = Identity.Domain.Entities.Users;

namespace Identity.Application.Tests.Users;

public sealed class AuthenticateUserTests
{
    [Fact]
    public async Task Authenticates_active_user_by_code()
    {
        var user = ActiveUser();
        var repository = new StubUsersRepository(user);
        var handler = new AuthenticateUserQueryHandler(
            repository,
            new StubPasswordHasher());

        var result = await handler.Handle(
            new AuthenticateUserQuery("AdMiN", "correct"),
            CancellationToken.None);

        Assert.Equal("admin", result.Code);
        Assert.Equal("admin@example.com", result.Email);
        Assert.Equal("AdMiN", repository.RequestedCode);
    }

    [Theory]
    [InlineData(false, false, "correct")]
    [InlineData(true, true, "correct")]
    [InlineData(true, false, "wrong")]
    public async Task Rejects_credentials_with_one_generic_message(
        bool active,
        bool deleted,
        string password)
    {
        var user = ActiveUser();
        user.IsActive = active;
        user.IsDeleted = deleted;
        var handler = new AuthenticateUserQueryHandler(
            new StubUsersRepository(user),
            new StubPasswordHasher());

        var error = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(
                new AuthenticateUserQuery("admin", password),
                CancellationToken.None));

        Assert.Equal("Invalid code or password.", error.Message);
    }

    private static UserEntity ActiveUser() =>
        new()
        {
            Id = 1,
            Code = "admin",
            Email = "admin@example.com",
            DisplayName = "Admin",
            PasswordHash = "hash",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

    private sealed class StubPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => "hash";

        public bool Verify(string password, string passwordHash) =>
            password == "correct" && passwordHash == "hash";
    }

    private sealed class StubUsersRepository(UserEntity user) : IUsersRepository
    {
        public string? RequestedCode { get; private set; }

        public Task AddAsync(UserEntity value, CancellationToken ct) =>
            Task.CompletedTask;

        public Task<UserEntity?> GetByIdAsync(ulong id, CancellationToken ct) =>
            Task.FromResult<UserEntity?>(user);

        public Task<UserEntity?> GetByCodeAsync(string code, CancellationToken ct)
        {
            RequestedCode = code;
            return Task.FromResult<UserEntity?>(user);
        }

        public Task<bool> CodeExistsAsync(
            string code,
            ulong? excludingId,
            CancellationToken ct) =>
            Task.FromResult(false);

        public Task<bool> EmailExistsAsync(
            string email,
            ulong? excludingId,
            CancellationToken ct) =>
            Task.FromResult(false);

        public Task UpdateAsync(UserEntity value, CancellationToken ct) =>
            Task.CompletedTask;

        public Task DeleteAsync(UserEntity value, CancellationToken ct) =>
            Task.CompletedTask;
    }
}
