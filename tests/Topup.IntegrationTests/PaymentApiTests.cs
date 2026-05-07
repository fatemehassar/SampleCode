using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Topup.IntegrationTests;

public class PaymentApiTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PaymentApiTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Purchase_Should_Return_Ok()
    {
        var request = new
        {
            amount = 1000,
            mobileNo = "09128083537",
            idempotencyKey = Guid.NewGuid().ToString()
        };

        var response = await _client.PostAsJsonAsync(
            "/api/payment/purchase",
            request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}