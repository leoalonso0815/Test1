using System.Globalization;

public static class NumberHelper
{
    public static string FormatInt2String(int num)
    {
        decimal currencyNum = num;
        if (currencyNum < 10000)
            return currencyNum.ToString(NumberFormatInfo.CurrentInfo);
        if (currencyNum >= 10000 && currencyNum < 1000000)
            return $"{currencyNum * (decimal)0.0001:0.#}万";
        if (currencyNum >= 1000000 && currencyNum < 10000000)
            return $"{currencyNum * (decimal)0.000001:0.#}百万";
        if (currencyNum >= 10000000 && currencyNum < 100000000)
            return $"{currencyNum * (decimal)0.0000001:0.#}千万";
        return $"{currencyNum * (decimal)0.00000001:0.#}亿";
    }
}