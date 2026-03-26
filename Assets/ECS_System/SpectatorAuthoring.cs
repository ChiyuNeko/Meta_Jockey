using Unity.Entities;
using UnityEngine;

// 這是掛在你傳統 Prefab 上的腳本
public class SpectatorAuthoring : MonoBehaviour
{
    public float randomSpeedOffset = 1.0f;

    // Baker 是負責將 GameObject 轉換為 Entity 的核心機制
    class Baker : Baker<SpectatorAuthoring>
    {
        public override void Bake(SpectatorAuthoring authoring)
        {
            // 取得當前 GameObject 轉換後的 Entity ID
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // 將我們定義的純資料 Component 加到這個 Entity 身上
            AddComponent(entity, new SpectatorData
            {
                RandomSpeedOffset = authoring.randomSpeedOffset
            });
        }
    }
}