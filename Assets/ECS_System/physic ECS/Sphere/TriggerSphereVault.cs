using Unity.Entities;
using UnityEngine;

// 1. 定義純資料：用來儲存球的藍圖 ID
public struct TriggerSphereVaultData : IComponentData
{
    public Entity SpherePrefab;
}

// 2. 烘焙器：掛在 Unity 場景的空物件上
public class TriggerSphereVault : MonoBehaviour
{
    public GameObject spherePrefab; // 在這裡拖入你的觸發球 Prefab

    class Baker : Baker<TriggerSphereVault>
    {
        public override void Bake(TriggerSphereVault authoring)
        {
            if (authoring.spherePrefab == null) return;

            Entity entity = GetEntity(TransformUsageFlags.None);
            
            // 將 GameObject Prefab 轉換為 Entity Prefab，並存入資料中
            AddComponent(entity, new TriggerSphereVaultData
            {
                SpherePrefab = GetEntity(authoring.spherePrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}