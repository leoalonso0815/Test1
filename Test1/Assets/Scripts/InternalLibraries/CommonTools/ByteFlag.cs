
public static class ByteFlag
{
    private static readonly int[] Flags = new int[]
    {
        0,
        1 << 0,
        1 << 1, 
        1 << 2, 
        1 << 3, 
        1 << 4, 
        1 << 5,
        1 << 6,
        1 << 7,
        1 << 8,
        1 << 9,
        1 << 10,
    };

    public static int GetFlags(int length)
    {
        int res = 0;
        for (int i = 0; i < length; i++)
            res = XORFlag(res, i);
        return res;
    }

    public static bool ContainsFlag(int value, int flagIndex)
    {
        if (flagIndex < 0 || flagIndex > Flags.Length)
            return false;
        return (value & Flags[flagIndex]) != 0;
    }

    public static int CombineFlag(int value, int index)
    {
        return value | Flags[index];
    }

    public static int XORFlag(int value, int flagIndex)
    {
        return value ^ Flags[flagIndex];
    }
}