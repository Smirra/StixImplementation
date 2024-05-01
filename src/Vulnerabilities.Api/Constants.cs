using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Vulnerabilities.Api;
public static class Constants
{
    public record Roles
    {
        public const string Editor = "Editor";
        public const string Reader = "Reader";
    }

    public readonly static OpenApiSecurityScheme SecurityScheme = new()
    {
        Description = "JWT Authorization header using the Bearer scheme.<br /><br />Input only the token.<br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        },
    };
}