namespace HMSMini.Web.Utilities;

public static class NumberToWordsConverter
{
    private static readonly string[] Ones =
    {
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    };

    private static readonly string[] Tens =
    {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

    public static string ConvertToIndianRupees(decimal amount)
    {
        if (amount == 0)
            return "Rupees Zero Only";

        var isNegative = amount < 0;
        amount = Math.Abs(amount);

        var rupees = (long)Math.Floor(amount);
        var paise = (int)Math.Round((amount - rupees) * 100);

        var result = "";

        if (rupees > 0)
        {
            result = "Rupees " + ConvertNumberToWords(rupees);
        }

        if (paise > 0)
        {
            if (rupees > 0)
                result += " and ";
            else
                result = "Rupees ";
            result += ConvertNumberToWords(paise) + " Paise";
        }

        if (string.IsNullOrEmpty(result))
            result = "Rupees Zero";

        if (isNegative)
            result = "Minus " + result;

        return result + " Only";
    }

    private static string ConvertNumberToWords(long number)
    {
        if (number == 0)
            return "";

        if (number < 0)
            return "Minus " + ConvertNumberToWords(-number);

        var parts = new List<string>();

        if (number / 10000000 > 0)
        {
            parts.Add(ConvertNumberToWords(number / 10000000) + " Crore");
            number %= 10000000;
        }

        if (number / 100000 > 0)
        {
            parts.Add(ConvertNumberToWords(number / 100000) + " Lakh");
            number %= 100000;
        }

        if (number / 1000 > 0)
        {
            parts.Add(ConvertNumberToWords(number / 1000) + " Thousand");
            number %= 1000;
        }

        if (number / 100 > 0)
        {
            parts.Add(ConvertNumberToWords(number / 100) + " Hundred");
            number %= 100;
        }

        if (number > 0)
        {
            if (number < 20)
            {
                parts.Add(Ones[number]);
            }
            else
            {
                var str = Tens[number / 10];
                if (number % 10 > 0)
                    str += " " + Ones[number % 10];
                parts.Add(str);
            }
        }

        return string.Join(" ", parts);
    }
}
