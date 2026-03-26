using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms; // 新增這行：為了操作 LocalTransform 讓球變大

// ==========================================
// 1. 觸發變色系統 (負責處理碰撞)
// ==========================================
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct TriggerColorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new ColorTriggerJob
        {
            GoofyLookup = SystemAPI.GetComponentLookup<GoofyAssTag>(true),
            TriggerLookup = SystemAPI.GetComponentLookup<TriggerSphereTag>(true), 
            DataLookup = SystemAPI.GetComponentLookup<GoofyAssData>(true),
            ColorLookup = SystemAPI.GetComponentLookup<EmissionColor>(false) 
        };

        state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public struct ColorTriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<GoofyAssTag> GoofyLookup;
    [ReadOnly] public ComponentLookup<TriggerSphereTag> TriggerLookup;
    [ReadOnly] public ComponentLookup<GoofyAssData> DataLookup;
    public ComponentLookup<EmissionColor> ColorLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity a = triggerEvent.EntityA;
        Entity b = triggerEvent.EntityB;

        if (GoofyLookup.HasComponent(a) && TriggerLookup.HasComponent(b)) ChangeColor(a);
        else if (GoofyLookup.HasComponent(b) && TriggerLookup.HasComponent(a)) ChangeColor(b);
    }

    private void ChangeColor(Entity goofyEntity)
    {
        // 取出這隻 GoofyAss 的資料
        var data = DataLookup[goofyEntity];

        // 把它的左手跟右手直接塗成 HitColor
        ColorLookup[data.SaberLeft] = new EmissionColor { Value = data.HitColor };
        ColorLookup[data.SaberRight] = new EmissionColor { Value = data.HitColor };
    }
}

// ==========================================
// 2. 觸發球擴散系統 (負責讓球變大)
// ==========================================
[BurstCompile]
public partial struct SphereGrowthSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // 尋找所有擁有 LocalTransform 和 GrowthRateData 的實體 (也就是我們的觸發球)
        foreach (var (transform, growthRate) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<GrowthRateData>>())
        {
            // 將原本的大小 (Scale) 加上 (速度 * 經過時間)
            transform.ValueRW.Scale += growthRate.ValueRO.Value * deltaTime;
        }
    }
}