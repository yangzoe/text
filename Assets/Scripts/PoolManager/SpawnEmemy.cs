using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string poolKey;   // 对象池键值（如"EnemyA"）
        public int weight = 1;   // 生成权重
    }

    [Header("生成设置")]
    public Vector2 maxPosition;
    public Vector2 minPosition;
    public List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("数量控制")]
    public float spawnInterval = 3f;
    public int initialMaxEnemies = 5;
    public float upgradeInterval = 30f;
    public int upgradeAmount = 2;
    public int maxEnemiesLimit = 20;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentMaxEnemies;
    private float upgradeTimer;

    void Start()
    {
        currentMaxEnemies = initialMaxEnemies;
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        // 定时提升最大数量
        upgradeTimer += Time.deltaTime;
        if (upgradeTimer >= upgradeInterval)
        {
            currentMaxEnemies = Mathf.Min(
                currentMaxEnemies + upgradeAmount,
                maxEnemiesLimit
            );
            upgradeTimer = 0f;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            TrySpawnEnemy();
        }
    }

    void TrySpawnEnemy()
    {
        // 清理已回收的敌人
        activeEnemies.RemoveAll(e => e == null || !e.activeSelf);

        if (activeEnemies.Count < currentMaxEnemies)
        {
            SpawnRandomEnemy();
        }
    }

    void SpawnRandomEnemy()
    {
        // 获取带权重随机类型
        string enemyKey = GetWeightedRandomKey();

        // 生成随机位置
        Vector2 spawnPos = new Vector2(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y)
        );

        // 从对象池获取实例
        GameObject enemy = ObjectPoolManager.Instance.GetObject(enemyKey);
        if (enemy != null)
        {
            enemy.transform.position = spawnPos;
            activeEnemies.Add(enemy);
        }
    }

    string GetWeightedRandomKey()
    {
        // 计算总权重
        int totalWeight = 0;
        foreach (EnemyType type in enemyTypes)
        {
            totalWeight += type.weight;
        }

        // 随机选择
        int random = Random.Range(0, totalWeight);
        foreach (EnemyType type in enemyTypes)
        {
            if (random < type.weight)
            {
                return type.poolKey;
            }
            random -= type.weight;
        }

        return enemyTypes[0].poolKey;
    }
}