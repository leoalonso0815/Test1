using UnityEngine;

[System.Serializable]
public struct Color4
{
    public Color c1;
    public Color c2;
    public Color c3;
    public Color c4;

    public Color4(Color c)
    {
        this.c1 = c;
        this.c2 = c;
        this.c3 = c;
        this.c4 = c;
    }
    
    public Color4(Color c1, Color c2, Color c3, Color c4)
    {
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
        this.c4 = c4;
    }

    /// <summary>
    ///   <para>Solid White. RGBA is (0, 0, 1, 1).</para>
    /// </summary>
    public static Color4 White => new Color4(Color.white);
}