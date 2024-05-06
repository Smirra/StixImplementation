using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Vulnerabilities.Api.Controllers;
using Vulnerabilities.Api.Tests;


namespace Vulnerabilities.Tests;
public class IntegrationTests(CustomWebApplicationFactory<Program> factory)
        : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task Post_SignInReader_IsSuccessful()
    {
        // Arrange
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/user/signin", new StringContent(JsonSerializer.Serialize(new SignInModel
        {
            Username = "reader",
            Password = "password"
        }), Encoding.UTF8, "application/json"));


        // Assert
        response.EnsureSuccessStatusCode();
    }

    
}
