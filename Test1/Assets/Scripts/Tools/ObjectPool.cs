using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; } // 单例实例

    [System.Serializable]
    public class Pool
    {
        public string tag; // 对象标识
        public GameObject prefab; // 预制体
        public int size = 20; // 初始池大小
        public Transform parentDefault; //初始父物体
    }

    public List<Pool> pools; // 对象池配置
    private Dictionary<string, Queue<GameObject>> poolDictionary; // 对象池字典

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 初始化对象池
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        for (var i = 0; i < pools.Count; i++)
        {
            var pool = pools[i];
            pool.parentDefault = new GameObject(pool.tag).transform;
            pool.parentDefault.SetParent(transform);
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int j = 0; j < pool.size; j++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(pool.parentDefault);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    
    /// <summary>
    /// 从池中获取对象
    /// </summary>
    /// <param name="poolTag"></param>
    /// <returns></returns>
    private GameObject GetObjectFromPool(string poolTag)
    {
        if (!poolDictionary.TryGetValue(poolTag, out var poolQueue))
        {
            Debug.LogWarning($"对象池中没有找到 {poolTag} 的配置");
            return null;
        }

        if (poolQueue.Count == 0)
        {
            ExpandPool(poolTag);
        }

        return poolQueue.Dequeue();
    }
    
    /// <summary>
    /// 设置父物体并显示
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parent"></param>
    private void PrepareObject(GameObject obj, Transform parent)
    {
        obj.transform.SetParent(parent);
        obj.SetActive(true);
    }

    /// <summary>
    /// 从池中获取对象（场景物体）
    /// </summary>
    /// <param name="poolTag"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string poolTag, Transform parent, Vector3 position, Quaternion rotation)
    {
        var objectToSpawn = GetObjectFromPool(poolTag);
        if (objectToSpawn == null) return null;
    
        PrepareObject(objectToSpawn, parent);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    /// <summary>
    /// 从池中获取对象(UIPrefab)
    /// </summary>
    /// <param name="poolTag"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string poolTag, Transform parent, Vector3 position)
    {
        var objectToSpawn = GetObjectFromPool(poolTag);
        if (objectToSpawn == null) return null;
    
        PrepareObject(objectToSpawn, parent);
        objectToSpawn.transform.position = position;

        return objectToSpawn;
    }
    
    /// <summary>
    /// 从池中获取对象(LocalPosition)
    /// </summary>
    /// <param name="poolTag"></param>
    /// <param name="parent"></param>
    /// <param name="localPosition"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject SpawnFromPoolToLocalPosition(string poolTag, Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        var objectToSpawn = GetObjectFromPool(poolTag);
        if (objectToSpawn == null) return null;
    
        PrepareObject(objectToSpawn, parent);
        objectToSpawn.transform.localPosition = localPosition;
        objectToSpawn.transform.localRotation = localRotation;

        return objectToSpawn;
    }

    /// <summary>
    /// 将对象返回池中
    /// </summary>
    /// <param name="poolTag"></param>
    /// <param name="objectToReturn"></param>
    public void ReturnToPool(string poolTag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(poolTag))
        {
            Debug.LogWarning($"对象池中没有找到 {poolTag} 的配置");
            return;
        }

        var pool = pools.Find(p => p.tag == poolTag);
        objectToReturn.transform.SetParent(pool != null ? pool.parentDefault : transform);
        objectToReturn.SetActive(false);
        poolDictionary[poolTag].Enqueue(objectToReturn);
    }

    /// <summary>
    /// 扩展对象池
    /// </summary>
    /// <param name="poolTag"></param>
    private void ExpandPool(string poolTag)
    {
        Pool targetPool = pools.Find(p => p.tag == poolTag);
        if (targetPool != null)
        {
            GameObject newObj = Instantiate(targetPool.prefab);
            newObj.SetActive(false);
            poolDictionary[poolTag].Enqueue(newObj);
        }
    }
}