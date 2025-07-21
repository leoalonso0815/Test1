using UnityEngine;

public static class ColorExtension
{
    public static string ToHtmlStringRGB(this Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }
}