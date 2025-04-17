using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string poolKey;   // ����ؼ�ֵ����"EnemyA"��
        public int weight = 1;   // ����Ȩ��
    }

    [Header("��������")]
    public Vector2 maxPosition;
    public Vector2 minPosition;
    public List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("��������")]
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
        // ��ʱ�����������
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
        // �����ѻ��յĵ���
        activeEnemies.RemoveAll(e => e == null || !e.activeSelf);

        if (activeEnemies.Count < currentMaxEnemies)
        {
            SpawnRandomEnemy();
        }
    }

    void SpawnRandomEnemy()
    {
        // ��ȡ��Ȩ���������
        string enemyKey = GetWeightedRandomKey();

        // �������λ��
        Vector2 spawnPos = new Vector2(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y)
        );

        // �Ӷ���ػ�ȡʵ��
        GameObject enemy = ObjectPoolManager.Instance.GetObject(enemyKey);
        if (enemy != null)
        {
            enemy.transform.position = spawnPos;
            activeEnemies.Add(enemy);
        }
    }

    string GetWeightedRandomKey()
    {
        // ������Ȩ��
        int totalWeight = 0;
        foreach (EnemyType type in enemyTypes)
        {
            totalWeight += type.weight;
        }

        // ���ѡ��
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