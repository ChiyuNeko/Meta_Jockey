using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

// 1. 定義 ECS 的陣列元素 (用來裝菜單上的各種球)
[InternalBufferCapacity(4)]
public struct SphereBlueprintElement : IBufferElementData
{
    public Entity Prefab;
}

// 2. 烘焙器
public class TriggerSphereVault : MonoBehaviour
{
    [Header("觸發球菜單")]
    [Tooltip("將各種不同顏色的 ECS 觸發球 Prefab 拖進這裡")]
    public List<GameObject> spherePrefabs = new List<GameObject>();

    class Baker : Baker<TriggerSphereVault>
    {
        public override void Bake(TriggerSphereVault authoring)
        {
            if (authoring.spherePrefabs == null || authoring.spherePrefabs.Count == 0) return;

            Entity entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<SphereBlueprintElement>(entity);
            
            // 把 Unity 的 List 轉換成 ECS 的清單
            foreach (var prefab in authoring.spherePrefabs)
            {
                if (prefab != null)
                {
                    buffer.Add(new SphereBlueprintElement
                    {
                        Prefab = GetEntity(prefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
}