using UnityEngine;

public class ExpansionSystem : MonoBehaviour
{
    [Header("自毀與擴散設定")]
    public float lifeTime = 3.0f; 
    public float growthSpeed = 5.0f; 

    // 內部計時器
    private float currentLifeTime = 0f;

    void Update()
    {
        // 1. 處理擴散 (每一幀往外膨脹)
        transform.localScale += Vector3.one * growthSpeed * Time.deltaTime;

        // 2. 處理生命週期 (計時器滿了就自我銷毀)
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}