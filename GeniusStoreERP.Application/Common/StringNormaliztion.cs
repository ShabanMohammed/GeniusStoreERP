using System.Text.RegularExpressions;

namespace GeniusStoreERP.Application.Common
{
    public static class StringExtensions
    {
        public static string? Sanitize(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input?.Trim();
            
            // التخلص من المسافات الزائدة أولاً
            string res = Regex.Replace(input.Trim(), @"\s+", " ");
            
            // توحيد الحروف المتشابهة (Normalizing Arabic)
            res = Regex.Replace(res, "[أإآ]", "ا");
            res = Regex.Replace(res, "ة", "ه");
            res = Regex.Replace(res, "[يى]", "ى"); // توحيد الياء بنقاط وبدون نقاط
            
            return res;
        }

        public static string? NormalizeArabic(this string? input) => input.Sanitize();
    }
}
