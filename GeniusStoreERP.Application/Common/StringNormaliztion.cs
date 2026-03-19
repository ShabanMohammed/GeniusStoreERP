using System.Text.RegularExpressions;

namespace GeniusStoreERP.Application.Common
{
    public static class StringExtensions
    {
        public static string? Sanitize(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input?.Trim();
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }
    }
}
