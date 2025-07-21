using UnityEngine;

public static class ColorTool
{
    public static Color GetColor(string _color)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#"+_color, out color);
        return color;
    }

    public static Color GetColor(int r, int g, int b, int a = 255)
    {
        Color color = new Color(r / 255, g / 255, b / 255, a / 255);
        return color;
    }
}
