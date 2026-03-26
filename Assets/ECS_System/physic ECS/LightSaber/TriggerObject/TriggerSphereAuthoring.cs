using Unity.Entities;
using UnityEngine;

using Unity.Entities;
using UnityEngine;

public class TriggerSphereAuthoring : MonoBehaviour
{
    [Header("自毀設定")]
    [Tooltip("觸發球生成後經過幾秒會自動銷毀？")]
    public float lifeTime = 3.0f; 

    [Header("擴散設定")]
    [Tooltip("每秒 Scale (大小) 增加多少？")]
    public float growthSpeed = 5.0f; // 預設每秒增加 5 的大小

    class Baker : Baker<TriggerSphereAuthoring>
    {
        public override void Bake(TriggerSphereAuthoring authoring)
        {
            // TransformUsageFlags.Dynamic 非常重要，這樣 ECS 才會允許它在遊戲中改變大小或位置
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // 貼上觸發球的標籤
            AddComponent<TriggerSphereTag>(entity);
            
            // 賦予壽命資料
            AddComponent(entity, new LifeTimeData { Value = authoring.lifeTime });

            // 賦予變大速度資料
            AddComponent(entity, new GrowthRateData { Value = authoring.growthSpeed });
        }
    }
}