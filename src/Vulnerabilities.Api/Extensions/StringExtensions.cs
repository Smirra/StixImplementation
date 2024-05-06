using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace Vulnerabilities.Api.Extensions;
public static class StringExtensions
{
    public static string PythonToDotNetStyle(this string str)
    {
        if (!str.IsNullOrEmpty())
        {
            return string.Concat(
                str.Split('_').Select(CultureInfo.CurrentCulture.TextInfo.ToTitleCase)
            );
        }

        return ""; 
    }
}
