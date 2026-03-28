using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

// ==========================================
// 1. 純資料與標籤 (Component Data)
// ==========================================

//public struct TriggerSphereTag : IComponentData {} 

// ==========================================
// 2. 烘焙器 (Authoring & Baker) - 掛在 Unity 物件上
// ==========================================
public struct GrowthRateData : IComponentData
{
    public float Value;
}
// ==========================================
// 3. 邏輯系統 (System) - 背景自動執行
// ==========================================
[BurstCompile]
public partial struct SelfDestructSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (lifeTime, entity) in SystemAPI.Query<RefRW<LifeTimeData>>().WithEntityAccess())
        {
            lifeTime.ValueRW.Value -= deltaTime;
            if (lifeTime.ValueRO.Value <= 0f)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}