using UnityEngine;
using System.Collections.Generic;

public class SpaceObjectManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] spacePrefabs;
    public int spawnCount = 100;
    public float spawnRadius = 50f;

    [Header("Warp Settings")]
    public float warpSpeedMultiplier = 20f;

    List<SpaceObject> objects = new List<SpaceObject>();

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = spacePrefabs[Random.Range(0, spacePrefabs.Length)];
            Vector3 pos = Random.insideUnitSphere * spawnRadius;

            GameObject obj = Instantiate(prefab, pos, Random.rotation, transform);
            SpaceObject so = obj.GetComponent<SpaceObject>();
            objects.Add(so);
        }
    }

    public void SetWarpMode(bool isWarp)
    {
        foreach (var obj in objects)
        {
            obj.SetWarp(isWarp, warpSpeedMultiplier);
        }
    }
}
