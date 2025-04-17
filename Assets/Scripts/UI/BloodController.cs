using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodController : MonoBehaviour
{
    public GameObject BloodPrefab;
    public Canvas canvas;
    public RectTransform initialPosition;
    private Vector3 LastPosition;
    public float xOffset = -100f;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    void Start()
    {
        LastPosition = initialPosition.anchoredPosition;
        for(int i = 0; i < 3; i++)     //设置初始个数
        {
            SpawnPrefab();
        }
    }
    
    public void SpawnPrefab()
    {
        Vector2 spawnPosition = new Vector2(LastPosition.x + xOffset, LastPosition.y );
        GameObject newPrefab = Instantiate(BloodPrefab, canvas.transform);
        RectTransform rectTransform = newPrefab.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(spawnPosition.x, spawnPosition.y);
        }

        spawnedPrefabs.Add(newPrefab);
        LastPosition = spawnPosition;
    }

    public void RemovePrefab()
    {
        GameObject lastPrefab = spawnedPrefabs[spawnedPrefabs.Count - 1];
        Destroy(lastPrefab);
        spawnedPrefabs.RemoveAt(spawnedPrefabs.Count - 1);
        if(spawnedPrefabs.Count > 0)
        {
            LastPosition = spawnedPrefabs[spawnedPrefabs.Count -1].GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            LastPosition = initialPosition.anchoredPosition;
        }
    }
}
