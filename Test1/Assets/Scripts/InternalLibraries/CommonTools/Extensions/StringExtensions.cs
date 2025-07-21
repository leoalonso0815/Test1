using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static int ParseToInt(this string str, int defaultValue = 0)
    {
        if (int.TryParse(str, out var result))
        {
            return result;
        }

        return defaultValue;
    }
    
    public static bool StartWithAny(this string str, List<string> pathList)
    {
        for (var i = 0; i < pathList.Count; i++)
        {
            var path = pathList[i];
            if (str.StartsWith(path))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsHasChinese(this string str)
    {
        var result = Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        return result;
    }

    /// <summary>
    /// 检查名字是否合法：字母，数字和下划线。
    /// </summary>
    /// <returns></returns>
    public static bool IsValidName(this string name)
    {
        return name.All(c => char.IsLetterOrDigit(c) || c.Equals('_')) && !name.IsHasChinese();
    }

    public static string RemoveValidName(this string nameWithValidChar)
    {
        var invalidChars = $"[{Regex.Escape(@"@!#$()., ")}]";
        var regex = new Regex(invalidChars);
        var newStr = regex.Replace(nameWithValidChar, string.Empty);
        return newStr;
    }
}