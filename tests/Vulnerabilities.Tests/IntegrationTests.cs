using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Vulnerabilities.Api.Controllers;
using Vulnerabilities.Api.Converters;
using Vulnerabilities.Api.Models;

namespace Vulnerabilities.Tests;

[TestCaseOrderer(
    ordererTypeName: "Vulnerabilities.Tests.PriorityOrderer",
    ordererAssemblyName: "Vulnerabilities.Tests")]
public class IntegrationTests(WebApplicationFactory<Program> factory)
        : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;
    private static HttpClient client = new();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new DateTimeConverter() }
    };

    private readonly VulnerabilityDTO testVulnPayload = new()
    {
        Name = "CVE-3001-123456",
        Description = "This is a test vulnerability",
        SpecVersion = "2.1",
        MagnussonSeverity = 11,
        Type = "vulnerability",
        Status = "open"
    };

    // Test and login the reader user. This test, of course need to run first.
    [Fact, TestPriority(0)]
    public async Task Post_SignInReader_IsSuccessfulAndReturnToken()
    {
        client = _factory.CreateClient();

        var response = await client.PostAsync("/user/signin", new StringContent(JsonSerializer.Serialize(new SignInModel
        {
            Username = "reader",
            Password = "password"
        }, _jsonOptions), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var token = DeserializeToken(content);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Assert.Contains(".", token);
    }

    [Fact, TestPriority(1)]
    public async Task Get_VulnerabilitiesAsReader_IsSuccessful()
    {
        var response = await client.GetAsync("/vulnerabilities");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var vulnerabilities = JsonSerializer.Deserialize<VulnRspWrapper<Vulnerability>>(content, _jsonOptions);

        Assert.NotEmpty(vulnerabilities!.Items!);
    }

    [Fact, TestPriority(1)]
    public async Task Get_VulnerabilitiesAsReaderWithFilter_IsSuccessful()
    {
        var response = await client.GetAsync("/vulnerabilities?filterField=magnusson_severity&filterValue=88");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var vulnerabilities = JsonSerializer.Deserialize<VulnRspWrapper<Vulnerability>>(content, _jsonOptions);

        Assert.NotEmpty(vulnerabilities!.Items!);

        foreach (var vuln in vulnerabilities.Items!)
        {
            Assert.Equal(88, vuln.MagnussonSeverity);
        }
    }

    [Fact, TestPriority(2)]
    public async Task Post_SignInEditor_IsSuccessfulAndReturnToken()
    {
        client = _factory.CreateClient();

        var response = await client.PostAsync("/user/signin", new StringContent(JsonSerializer.Serialize(new SignInModel
        {
            Username = "editor",
            Password = "password"
        }, _jsonOptions), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var token = DeserializeToken(content);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token ?? "");

        Assert.Contains(".", token);
    }


    [Fact, TestPriority(3)]
    public async Task Post_CreateVulnerabilityAsEditor_IsSuccessful()
    {
        var response = await client.PostAsync("/vulnerabilities", new StringContent(JsonSerializer.Serialize(testVulnPayload,
                                                                                                             _jsonOptions), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var vulnerability = JsonSerializer.Deserialize<Vulnerability>(content, _jsonOptions);

        Assert.NotNull(vulnerability);
        Assert.Equal("CVE-2001-123456", vulnerability!.Name);
    }


    private static string DeserializeToken(string content)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(content)?["access_token"] ?? "";
    }
}