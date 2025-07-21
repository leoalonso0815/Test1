using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class LinqExtensions
{
    /// <summary>
    /// 移除List[0]并返回该元素。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="popout"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T PopFirst<T>(this List<T> list)
    {
        if (null == list || 0 == list.Count)
        {
            return default(T);
        }

        T obj = list[0];
        list.RemoveAt(0);
        return obj;
    }

    /// <summary>
    /// 把List当队列用：进队列。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void EnqueueList<T>(this List<T> list, T item)
    {
        if (null == list)
        {
            list = new List<T>();
        }

        list.Insert(list.Count, item);
    }

    /// <summary>
    /// 把List当队列用：从队首插队。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void PushList<T>(this List<T> list, T item)
    {
        if (null == list)
        {
            list = new List<T>();
        }

        list.Insert(0, item);
    }

    /// <summary>
    /// 把List当队列用：从index位置插队。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <param name="i"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertList<T>(this List<T> list, T item, int i)
    {
        if (null == list)
        {
            list = new List<T>();
        }

        if (i > list.Count)
        {
            Debug.LogError(string.Format("error insert index {0} to Count {1}", i, list.Count));
            i = list.Count;
        }

        list.Insert(i, item);
    }

    /// <summary>
    /// 把List当队列用，出队列。
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T DequeueList<T>(this List<T> list)
    {
        return PopFirst(list);
    }

    public static T PeekList<T>(this List<T> list)
    {
        return list.FirstOrDefault();
    }

    public static bool IsNullOrSmaller<T>(this List<T> list, int length = 1)
    {
        return null == list || list.Count < length;
    }

    public static bool IsNullOrEmpty<T>(this List<T> list, int length = 1)
    {
        return IsNullOrSmaller(list);
    }

    public static bool IsNullOrSmaller<T>(this T[] array, int length = 1)
    {
        return null == array || array.Length < length;
    }

    public static bool IsNullOrEmpty<T>(this T[] array, int length = 1)
    {
        return IsNullOrSmaller(array);
    }

    public static T RandomItem<T>(this List<T> list)
    {
        return null == list ? default : list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 安全的根据index获取Item，不会报空。
    /// 取不到会返回默认值。
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index">位置序号</param>
    /// <param name="defaultValue">取不到时应返回的默认值</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetItemSafely<T>(this List<T> array, int index, T defaultValue = default)
    {
        if (null == array)
        {
            return defaultValue;
        }

        if (index >= 0 && index < array.Count)
        {
            return array[index];
        }

        return defaultValue;
    }

    public static bool TryGetItem<T>(this T[] array, int index, out T result)
    {
        if (null != array && index >= 0 && index < array.Length)
        {
            result = array[index];
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// 安全的根据index获取Item，不会报空。
    /// 取不到会返回默认值。
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index">位置序号</param>
    /// <param name="defaultValue">取不到时应返回的默认值</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetItemSafely<T>(this T[] array, int index, T defaultValue = default)
    {
        if (null == array)
        {
            return defaultValue;
        }

        if (index >= 0 && index < array.Length)
        {
            return array[index];
        }

        return defaultValue;
    }

    /// <summary>
    /// eg:
    /// Dictionary<int, int> itemAndEquipDictionary = null;
    /// TBItem.DataList.ToRefDictionary(k => k.Id, v => Item.__ID__, ref itemAndEquipDictionary);
    /// TBEquip.DataList.ToRefDictionary(k => k.Id, v => Item.__ID__, ref itemAndEquipDictionary);
    /// </summary>
    /// <param name="sources"></param>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="dictionary"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public static void ToRefDictionary<TSource, TKey, TElement>(
        this IEnumerable<TSource> sources,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector, ref Dictionary<TKey, TElement> dictionary)
    {
        if (sources == null || keySelector == null || elementSelector == null)
        {
            return;
        }

        if (dictionary == null)
        {
            dictionary = new Dictionary<TKey, TElement>();
        }

        foreach (var source1 in sources)
        {
            var key = keySelector(source1);
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, elementSelector(source1));
            }
        }
    }

    public static HashSet<T> ToHashSet<T>(
        this IEnumerable<T> source,
        IEqualityComparer<T> comparer = null)
    {
        return new HashSet<T>(source, comparer);
    }
}