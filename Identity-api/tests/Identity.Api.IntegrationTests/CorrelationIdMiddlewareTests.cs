using Identity.Api.Middleware;
using Microsoft.AspNetCore.Http;

namespace Identity.Api.IntegrationTests;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_PreservesValidIncomingCorrelationId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "client-request-123";
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.Equal("client-request-123", context.TraceIdentifier);
        Assert.Equal("client-request-123", context.Response.Headers[CorrelationIdMiddleware.HeaderName]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("contains a space")]
    [InlineData("contains\r\nnewline")]
    public async Task InvokeAsync_ReplacesMissingOrUnsafeCorrelationId(string value)
    {
        var context = new DefaultHttpContext();
        if (value.Length > 0)
            context.Request.Headers[CorrelationIdMiddleware.HeaderName] = value;
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        var generated = context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        Assert.NotEmpty(generated);
        Assert.NotEqual(value, generated);
        Assert.Equal(generated, context.TraceIdentifier);
    }
}
