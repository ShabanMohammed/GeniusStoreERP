using System.Text;

namespace GeniusStoreERP.Application.Common;

public static class CurrencyToWordsHelper
{
    private static readonly string[] Ones = { "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة", "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر" };
    private static readonly string[] Tens = { "", "عشرة", "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون" };
    private static readonly string[] Hundreds = { "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة" };
    private static readonly string[] Thousands = { "", "ألف", "ألفان", "آلاف", "ألف" };
    private static readonly string[] Millions = { "", "مليون", "مليونان", "ملايين", "مليون" };

    public static string ConvertToArabic(decimal amount, string currencyName = "جنيه", string subunitName = "قرش")
    {
        if (amount == 0) return "صفر " + currencyName;

        long integerPart = (long)Math.Floor(amount);
        int fractionalPart = (int)(Math.Round(amount - integerPart, 2) * 100);

        StringBuilder sb = new StringBuilder();

        if (integerPart > 0)
        {
            sb.Append(NumberToArabic((ulong)integerPart));
            sb.Append(" ");
            sb.Append(currencyName);
        }

        if (fractionalPart > 0)
        {
            if (integerPart > 0) sb.Append(" و ");
            sb.Append(NumberToArabic((ulong)fractionalPart));
            sb.Append(" ");
            sb.Append(subunitName);
        }

        sb.Append(" فقط لا غير");
        return sb.ToString();
    }

    private static string NumberToArabic(ulong number)
    {
        if (number == 0) return "";
        if (number < 20) return Ones[number];
        if (number < 100)
        {
            ulong ones = number % 10;
            ulong tens = number / 10;
            return (ones > 0 ? Ones[ones] + " و " : "") + Tens[tens];
        }
        if (number < 1000)
        {
            ulong hundreds = number / 100;
            ulong remainder = number % 100;
            return Hundreds[hundreds] + (remainder > 0 ? " و " + NumberToArabic(remainder) : "");
        }
        if (number < 1000000)
        {
            ulong thousands = number / 1000;
            ulong remainder = number % 1000;
            string thousandsStr;
            if (thousands == 1) thousandsStr = "ألف";
            else if (thousands == 2) thousandsStr = "ألفان";
            else if (thousands >= 3 && thousands <= 10) thousandsStr = NumberToArabic(thousands) + " آلاف";
            else thousandsStr = NumberToArabic(thousands) + " ألف";
            
            return thousandsStr + (remainder > 0 ? " و " + NumberToArabic(remainder) : "");
        }
        if (number < 1000000000)
        {
            ulong millions = number / 1000000;
            ulong remainder = number % 1000000;
            string millionsStr;
            if (millions == 1) millionsStr = "مليون";
            else if (millions == 2) millionsStr = "مليونان";
            else if (millions >= 3 && millions <= 10) millionsStr = NumberToArabic(millions) + " ملايين";
            else millionsStr = NumberToArabic(millions) + " مليون";

            return millionsStr + (remainder > 0 ? " و " + NumberToArabic(remainder) : "");
        }

        return number.ToString(); // Fallback for extremely large numbers
    }
}
