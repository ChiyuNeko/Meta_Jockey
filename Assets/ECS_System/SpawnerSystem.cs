using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (spawner, entity) in SystemAPI.Query<RefRO<SpawnerData>>().WithEntityAccess())
        {
            var instances = new NativeArray<Entity>(spawner.ValueRO.SpawnCount, Allocator.Temp);
            state.EntityManager.Instantiate(spawner.ValueRO.Prefab, instances);

            uint randomSeed = (uint)(SystemAPI.Time.ElapsedTime * 100000) + 1;
            var random = Unity.Mathematics.Random.CreateFromIndex(randomSeed);
            
            float radius = spawner.ValueRO.SpawnRadius;
            float3 center = spawner.ValueRO.CenterPosition;
            float scale = spawner.ValueRO.ScaleFactor;
            quaternion baseRotation = spawner.ValueRO.BaseRotationOffset;
            quaternion spawnerRotation = spawner.ValueRO.SpawnerRotation;

            foreach (var instance in instances)
            {
                // 1. 產生均勻分佈的半徑 (開根號讓分佈平均)
                float randomDist = math.sqrt(random.NextFloat(0f, 1f)) * radius;

                // 2. 【關鍵修改】隨機角度：0 到 360 度 (math.PI * 2f 代表 360 度)
                float randomAngle = random.NextFloat(0f, math.PI * 2f);

                // 3. 極座標轉直角座標 (X, Z)，產生一個完整的圓形分佈
                float3 localOffset = new float3(
                    math.cos(randomAngle) * randomDist,
                    0,
                    math.sin(randomAngle) * randomDist
                );

                // 4. 根據 Spawner 的旋轉方向轉向 (雖然是正圓，但這能確保如果人群有基礎面向，會跟著轉)
                float3 rotatedOffset = math.rotate(spawnerRotation, localOffset);
                
                // 加上中心點座標
                float3 finalPosition = center + rotatedOffset;

                state.EntityManager.SetComponentData(instance, LocalTransform.FromPositionRotationScale(
                    finalPosition, 
                    baseRotation, 
                    scale
                ));
            }

            instances.Dispose();
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}