using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomFood : MonoBehaviour
{
    public Vector3 center;
    public GameObject hawkPrefab;
    public GameObject dovePrefab;
    public GameObject foodPrefab;
    public int min;
    public int max;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void Btn_Start_Click()
    {
        for (int i = 0; i < 100; i++)
        {
            spawnPrefab(hawkPrefab);
            spawnPrefab(dovePrefab);
            spawnPrefab(foodPrefab);
        }
    }

    public void spawnPrefab(GameObject prefab)
    {
        Vector3 pos = center + new Vector3(Random.Range(min, max), Random.Range(min, max), 0);

        Instantiate(prefab, pos, Quaternion.identity);
    }
}
