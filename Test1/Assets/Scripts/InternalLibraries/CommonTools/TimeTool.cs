using System;
using UnityEngine;

/// <summary>
/// 时间换算
/// </summary>
public static class TimeTool
{
    private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static TimeSpan GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - timeStampStartTime;
        return ts;
    }

    /// <summary>
    /// DateTime转换为10位时间戳（单位：秒）
    /// </summary>
    /// <param name="dateTime"> DateTime</param>
    /// <returns>10位时间戳（单位：秒）</returns>
    public static long DateTimeToTimeStamp(DateTime dateTime)
    {
        return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalSeconds;
    }

    public static int MinuteToSecond(int minute)
    {
        return minute * 60;
    }

    /// <summary>
    /// 是否是今天
    /// </summary>
    public static bool IsToday(long timeInfo)
    {
        DateTime dateTimeInfo = TimeStampToDateTime(timeInfo);
        if (DateTime.Today == dateTimeInfo.Date)
        {
            return true;
        }
        return false;
    }

    public static int GetTimeWeek(DateTime _dateTime)
    {
        DayOfWeek kDayOfWeek = _dateTime.DayOfWeek;
        int nWeekNum = ((int)kDayOfWeek + 6) % 7 + 1;
        return nWeekNum;
    }

    /// <summary>
    /// 获取间隔天数
    /// </summary>
    public static int GetIntervalDay(long _one, long _two)
    {
        long lDayNumOne = _one / 86400; //原点总天数,大
        long lDayNumTwo = _two / 86400; //
        int nIntervalDay = 0;
        if (_one > _two)
        {
            nIntervalDay = (int)(lDayNumOne - lDayNumTwo);
        }
        else
        {
            nIntervalDay = (int)(lDayNumTwo - lDayNumOne);
        }
        return nIntervalDay;
    }

    /// <summary>
    /// 本地时间
    /// </summary>
    /// <returns></returns>
    public static long GetNowTimeForLong()
    {
        return DateTimeToTimeStamp(DateTime.Now);
    }

    /// <summary>
    /// 10位时间戳（单位：秒）转换为DateTime
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <param name="inMilli">是否是毫秒</param>
    /// <returns></returns>
    public static DateTime TimeStampToDateTime(long timeStamp, bool inMilli = false)
    {
        DateTimeOffset dateTimeOffset = inMilli
            ? DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
            : DateTimeOffset.FromUnixTimeSeconds(timeStamp);
        return dateTimeOffset.LocalDateTime;
    }

    /// <summary>
    /// 秒转小时
    /// </summary>
    public static int OnSecondConvertedToHours(int _second)
    {
        int h = Mathf.FloorToInt(_second / 3600f);
        return h;
    }

    /// <summary>
    /// 时间格式化
    /// </summary>
    public static string OnConvertedToWatch(long _time, int _type = 1)
    {
        if (_time <= 0)
        {
            return "0";
        }
        float h = Mathf.FloorToInt(_time / 3600f);
        float m = Mathf.FloorToInt(_time / 60f - h * 60f);
        float s = Mathf.FloorToInt(_time - m * 60f - h * 3600f);
        //Debug.LogWarning("h="+h+"m="+m+"s="+s);
        switch (_type)
        {
            case 1:
                return EndDefWatch(h, m, s); //EndOneWatch(h, m, s);
            case 2:
                return EndTwoWatch(h, m, s);
            case 3:
                return EndNumeralWatch(h, m, s);
        }
        return "";
    }

    private static string EndNumeralWatch(float _h, float _m, float _s)
    {
        string szH = string.Format("{0:00}:{1:00}:{2:00}", _h, _m, _s);
        return szH;
    }

    private static string EndTwoWatch(float _h, float _m, float _s)
    {
        string zH = _h + "h";
        string zM = _m + "m";
        string zS = _s + "s";
        string szEndTime = "";
        if (_h != 0)
        {
            szEndTime += zH;
        }
        if (_m != 0)
        {
            szEndTime += zM;
        }
        if (_s != 0)
        {
            szEndTime += zS;
        }
        return szEndTime;
    }

    //a) 有小时的时候，显示小时和分钟，即 2h30m
    //b) 当没有小时的时候，显示分钟和秒，即 2m30s
    private static string EndDefWatch(float _h, float _m, float _s)
    {
        string zH = _h + "h";
        string zM = _m + "m";
        string zS = _s + "s";
        string szEndTime = "";
        if (_h != 0)
        {
            szEndTime += zH;
            szEndTime += zM;
            return szEndTime;
        }
        else
        {
            szEndTime += zM;
            szEndTime += zS;
            return szEndTime;
        }
    }

    /// <summary>
    /// 时间格式化
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static string TimeFormatInternal(double seconds, TimeFormatLevel level)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        char[] span = default;
        int readIndex = 0;
        switch (level)
        {
            case TimeFormatLevel.D:
                span = string.Intern("00天").ToCharArray();
                SpanReader(span, timeSpan.Days, readIndex);
                break;
            case TimeFormatLevel.H:
                span = string.Intern("00h").ToCharArray();
                SpanReader(span, timeSpan.Hours, readIndex);
                break;
            case TimeFormatLevel.M:
                span = string.Intern("00m").ToCharArray();
                SpanReader(span, timeSpan.Minutes, readIndex);
                break;
            case TimeFormatLevel.S:
                span = string.Intern("00s").ToCharArray();
                SpanReader(span, timeSpan.Seconds, readIndex);
                break;
            case TimeFormatLevel.DH:
                span = string.Intern("00天 00h").ToCharArray();
                SpanReader(span, timeSpan.Days, readIndex);
                readIndex += 4;
                SpanReader(span, timeSpan.Hours, readIndex);
                break;
            case TimeFormatLevel.HM:
                span = string.Intern("00h00m").ToCharArray();
                SpanReader(span, timeSpan.Hours, readIndex);
                readIndex += 3;
                SpanReader(span, timeSpan.Minutes, readIndex);
                break;
            case TimeFormatLevel.MS:
                span = string.Intern("00分00秒").ToCharArray();
                SpanReader(span, timeSpan.Minutes, readIndex);
                readIndex += 3;
                SpanReader(span, timeSpan.Seconds, readIndex);
                break;
            case TimeFormatLevel.DHM:
                span = string.Intern("00天 00:00").ToCharArray();
                SpanReader(span, timeSpan.Days, readIndex);
                readIndex += 4;
                SpanReader(span, timeSpan.Hours, readIndex);
                SpanReader(span, timeSpan.Minutes, readIndex + 3);
                break;
            case TimeFormatLevel.HMS:
                span = string.Intern("00:00:00").ToCharArray();
                SpanReader(span, timeSpan.Hours, readIndex);
                readIndex += 3;
                SpanReader(span, timeSpan.Minutes, readIndex);
                readIndex += 6;
                SpanReader(span, timeSpan.Seconds, readIndex);
                break;
            case TimeFormatLevel.DHMs:
                span = string.Intern("00天 00:00:00").ToCharArray();
                SpanReader(span, timeSpan.Days, readIndex);
                readIndex += 4;
                SpanReader(span, timeSpan.Hours, readIndex);
                SpanReader(span, timeSpan.Minutes, readIndex + 3);
                SpanReader(span, timeSpan.Seconds, readIndex + 6);
                break;
            case TimeFormatLevel.AUTO:
                if (timeSpan.Days > 0)
                {
                    span = string.Intern("00天 00:00:00").ToCharArray();
                    SpanReader(span, timeSpan.Days, readIndex);
                    readIndex += 4;
                    SpanReader(span, timeSpan.Hours, readIndex);
                    SpanReader(span, timeSpan.Minutes, readIndex + 3);
                    SpanReader(span, timeSpan.Seconds, readIndex + 6);
                }
                else if (timeSpan.Hours > 0)
                {
                    span = string.Intern("00时00分").ToCharArray();
                    SpanReader(span, timeSpan.Hours, readIndex);
                    readIndex += 3;
                    SpanReader(span, timeSpan.Minutes, readIndex);
                }
                else
                {
                    span = string.Intern("00分00秒").ToCharArray();
                    SpanReader(span, timeSpan.Minutes, readIndex);
                    readIndex += 3;
                    SpanReader(span, timeSpan.Seconds, readIndex);
                }
                break;
        }
        return new string(span);
    }

    private static void SpanReader(char[] span, decimal value, int startIndex)
    {
        if (value >= 10)
        {
            span[startIndex] = (char)(decimal.Truncate(value / 10) + 48);
            span[startIndex + 1] = (char)(decimal.Remainder(value, 10) + 48);
        }
        else
        {
            decimal v = value + 48;
            if (v < char.MinValue)
                v = char.MinValue;
            span[startIndex + 1] = (char)v;
        }
    }

    /// <summary>
    /// 好友离线时间表述
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string GetFriendOfflineTimeDescription(long seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        if (timeSpan.Days >= 7)
            return "1周";
        if (timeSpan.Days >= 1)
            return $"{timeSpan.Days}天";
        if (timeSpan.Hours >= 1)
            return $"{timeSpan.Hours}小时";
        if (timeSpan.Minutes >= 1)
            return $"{timeSpan.Minutes}分钟";
        return "1分钟";
    }
    
}

public enum TimeFormatLevel
{
    // 00 天
    D = 1 << 0,

    // 00 H
    H = 1 << 1,

    // 00 M
    M = 1 << 2,

    // 00 S
    S = 1 << 3,

    // 0D0H
    DH = 1 << 4,

    // 0H0M
    HM = 1 << 5,

    // 0M0S
    MS = 1 << 6,

    // 00天 00:00
    DHM = 1 << 7,

    // 00:00:00
    HMS = 1 << 8,

    // 00天 00:00:00
    DHMs = 1 << 9,

    // 自动(按照时间统一规则) 最大单位 天
    AUTO = 1 << 10,
}