using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct TriggerLightSaberSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new LightSaberTriggerJob
        {
            // 只依賴 LightSaberData.cs 裡面定義好的標籤與資料
            SaberLookup = SystemAPI.GetComponentLookup<LightSaberTag>(true),
            TriggerLookup = SystemAPI.GetComponentLookup<LightSaberTriggerTag>(true), 
            DataLookup = SystemAPI.GetComponentLookup<LightSaberData>(false),
            ColorLookup = SystemAPI.GetComponentLookup<EmissionColor>(false)
        };

        state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public struct LightSaberTriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<LightSaberTag> SaberLookup;
    [ReadOnly] public ComponentLookup<LightSaberTriggerTag> TriggerLookup;
    public ComponentLookup<LightSaberData> DataLookup;
    public ComponentLookup<EmissionColor> ColorLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity a = triggerEvent.EntityA;
        Entity b = triggerEvent.EntityB;

        if (SaberLookup.HasComponent(a) && TriggerLookup.HasComponent(b))
            ApplyHit(a);
        else if (SaberLookup.HasComponent(b) && TriggerLookup.HasComponent(a))
            ApplyHit(b);
    }

    private void ApplyHit(Entity entity)
    {
        var data = DataLookup[entity];
        data.CurrentTimer = data.HitDuration; 
        DataLookup[entity] = data;

        ColorLookup[entity] = new EmissionColor { Value = data.HitColor };
    }
}