using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag; // 对象池的标签 (例如 "Enemy", "PlayerBullet", "EnemyBullet")
        public GameObject prefab; // 该对象池对应的预制体
        public int size; // 对象池的初始大小
    }

    public static ObjectPooler Instance; // 单例实例

    public List<Pool> pools; // 可以配置多个不同的对象池
    public Dictionary<string, Queue<GameObject>> poolDictionary; // 存储实际的对象队列

    private void Awake()
    {
        Instance = this; // 设置单例
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            var objectQueue = new Queue<GameObject>();

            for (var i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab);
                obj.SetActive(false); // 初始时禁用对象
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectQueue);
            Debug.Log($"Pool with tag '{pool.tag}' created with {pool.size} objects.");
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        var queue = poolDictionary[tag];

        if (queue.Count == 0)
        {
            // 可选：如果池中对象不够用，可以动态创建新的对象
            // 找到对应的 Pool 配置来获取 prefab
            var poolToExpand = pools.Find(p => p.tag == tag);
            if (poolToExpand != null)
            {
                Debug.LogWarning($"Pool with tag '{tag}' is empty. Expanding pool.");
                var newObj = Instantiate(poolToExpand.prefab);
                // newObj.SetActive(false); // 立即使用，所以不需要先禁用
                // queue.Enqueue(newObj); // 不把它放回队列，直接用
                // 新创建的对象通常需要一些初始化，这里直接激活并设置位置旋转
                newObj.transform.position = position;
                newObj.transform.rotation = rotation;
                newObj.SetActive(true);
                // 注意：这种动态扩展的对象在“返回”时也应该能被正确处理
                // 或者，你可以选择不扩展，直接返回null或一个标记对象
                return newObj;
            }
            else
            {
                Debug.LogError($"Could not find pool config for tag '{tag}' to expand.");
                return null;
            }
        }

        var objectToSpawn = queue.Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // 可选: 调用对象上的重置方法 (如果对象有的话)
        // IPoolable pooledObj = objectToSpawn.GetComponent<IPoolable>();
        // if (pooledObj != null)
        // {
        //     pooledObj.OnObjectSpawn();
        // }

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist for returning object.");
            // 也可以选择销毁这个不属于任何已知池的对象
            // Destroy(objectToReturn);
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}