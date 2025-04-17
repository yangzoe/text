using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ObjectPoolManager : MonoBehaviour
{
    public int num = 0;
    public static ObjectPoolManager Instance;
    private Player player;
    [System.Serializable]
    public class Pool
    {
        // 预制体名称
        public string key;      
        public GameObject prefab;
        public int initialSize;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools(); // 初始化对象池
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.key, objectPool);
        }
    }

    public GameObject GetObject(string key)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError("Pool key " + key + " not found!");
            return null;
        }

        if (poolDictionary[key].Count == 0)
        {
            // 动态扩容
            Pool pool = pools.Find(p => p.key == key);
            GameObject obj = Instantiate(pool.prefab);
            poolDictionary[key].Enqueue(obj);
        }

        GameObject instance = poolDictionary[key].Dequeue();
        instance.SetActive(true);

        return instance;
    }

    public void ReturnObject(string key, GameObject obj)
    {
        if(obj.tag != "Bullet")
        {
            Debug.Log(obj.name);
            num++;
        }
        obj.SetActive(false);
        poolDictionary[key].Enqueue(obj);
    }   

}