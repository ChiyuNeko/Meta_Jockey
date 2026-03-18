using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 確保掛載此腳本的物件上一定有 Collider 元件
[RequireComponent(typeof(Collider))]
public class SpawnCrowd : MonoBehaviour
{
    [Header("生成設定")]
    [Tooltip("請將你想生成的 Prefab 拖曳到這裡")]
    public GameObject prefabToSpawn; 
    
    [Tooltip("要生成的總數量")]
    public int spawnCount = 10;      

    private Collider spawnArea;

    void Start()
    {
        // 取得物件上的 Collider 元件
        spawnArea = GetComponent<Collider>();

        // 防呆機制：確保你有放入 Prefab
        if (prefabToSpawn == null)
        {
            Debug.LogError("SpawnCrowd: 你還沒有指定要生成的 Prefab 喔！");
            return;
        }

        // 執行生成陣列
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        // 取得這個 Collider 的邊界 (Bounding Box)
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < spawnCount; i++)
        {
            // 在邊界的最小值與最大值之間，隨機決定 X, Y, Z 座標
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            // 組合出最終的隨機位置
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // 生成指定的 Prefab (位置為剛剛算出的隨機位置，旋轉角度為預設的不旋轉)
            // 如果你想把生成的物件設為這個腳本所在物件的子物件，可以在後面加上 transform
            // 例: Instantiate(prefabToSpawn, randomPosition, Quaternion.identity, transform);
            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
        }
    }
}