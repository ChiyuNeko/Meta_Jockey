using UnityEngine;
using System.Collections.Generic;

public class DebrisSpawner : MonoBehaviour
{
    [System.Serializable]
    public class DebrisEntry
    {
        public GameObject prefab;
        [Range(0f, 100f)]
        public float weight = 10f; // 出現權重
    }

    [Header("Debris List (With Probability)")]
    public List<DebrisEntry> debrisList = new List<DebrisEntry>();

    [Header("Spawn Settings")]
    public int count = 200;
    public Transform spawnAreaMin;
    public Transform spawnAreaMax;

    private float totalWeight;

    void Start()
    {
        CalculateTotalWeight();

        for (int i = 0; i < count; i++)
            Spawn();
    }

    void CalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (var entry in debrisList)
            totalWeight += entry.weight;
    }

    void Spawn()
    {
        if (debrisList.Count == 0 || totalWeight <= 0f)
            return;

        GameObject prefab = GetRandomDebrisByWeight();

        Vector3 pos = new Vector3(
            Random.Range(spawnAreaMin.position.x, spawnAreaMax.position.x),
            Random.Range(spawnAreaMin.position.y, spawnAreaMax.position.y),
            Random.Range(spawnAreaMin.position.z, spawnAreaMax.position.z)
        );

        GameObject debris = Instantiate(prefab, pos, Quaternion.identity);

        // 把生成範圍傳給垃圾本體
        SpaceDebris sd = debris.GetComponent<SpaceDebris>();
        if (sd != null)
        {
            sd.spawnAreaMin = spawnAreaMin;
            sd.spawnAreaMax = spawnAreaMax;
        }
    }

    GameObject GetRandomDebrisByWeight()
    {
        float random = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var entry in debrisList)
        {
            current += entry.weight;
            if (random <= current)
                return entry.prefab;
        }

        // 保底（理論上不會走到）
        return debrisList[0].prefab;
    }
}
