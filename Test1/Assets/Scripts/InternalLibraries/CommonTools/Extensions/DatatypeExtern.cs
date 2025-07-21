using System;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 Round(this Vector3 vector, int pos)
    {
        float vecX = (float)Math.Round(vector.x, pos);
        float vecY = (float)Math.Round(vector.y, pos);
        float vecZ = (float)Math.Round(vector.z, pos);
        Vector3 newVec = new Vector3(vecX,vecY,vecZ);
        return newVec;
    }

    /// <summary>
    /// 转化为万分比小数。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float ToTenThousandthRatio(this int value)
    {
        return value * 0.0001f;
    }
}

public class NumberExtensions
{
    public static int GetNextInCirculation(int current, int minInclude, int maxExclude)
    {
        var result = current + 1;
        result = result >= maxExclude ? minInclude : result;
        return result;
    }
}