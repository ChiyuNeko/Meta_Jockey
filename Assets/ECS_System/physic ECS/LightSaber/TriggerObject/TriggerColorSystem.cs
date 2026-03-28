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
            DataLookup = SystemAPI.GetComponentLookup<GoofyAssData>(false),
            ColorLookup = SystemAPI.GetComponentLookup<EmissionColor>(false), 
            EffectLookup = SystemAPI.GetComponentLookup<SphereEffectData>(true)
        };


        var job2 = new GoofyTimerJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            // 開放權限讓工人可以修改顏色
            ColorLookup = SystemAPI.GetComponentLookup<EmissionColor>(false)
        };
        job2.ScheduleParallel();
        state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]


public struct ColorTriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<GoofyAssTag> GoofyLookup;
    [ReadOnly] public ComponentLookup<TriggerSphereTag> TriggerLookup;
    
    // 👇 新增這行：讓我們可以讀取球身上的顏料資料
    [ReadOnly] public ComponentLookup<SphereEffectData> EffectLookup; 
    
    public ComponentLookup<GoofyAssData> DataLookup; 
    public ComponentLookup<EmissionColor> ColorLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity a = triggerEvent.EntityA;
        Entity b = triggerEvent.EntityB;

        // 判斷誰是觀眾(Goofy)，誰是球(Sphere)
        if (GoofyLookup.HasComponent(a) && TriggerLookup.HasComponent(b)) 
            ChangeColor(a, b); // a是觀眾, b是球
        else if (GoofyLookup.HasComponent(b) && TriggerLookup.HasComponent(a)) 
            ChangeColor(b, a); // b是觀眾, a是球
    }

    private void ChangeColor(Entity goofyEntity, Entity sphereEntity)
    {
        var data = DataLookup[goofyEntity];
        
        // 讀出這顆球專屬的特效設定！
        var sphereEffect = EffectLookup[sphereEntity];

        if (data.LastHitSphere == sphereEntity) return;

        // 👇 核心動作：把球的特效資料「複製」到觀眾的大腦裡暫存
        data.ActiveHitColor = sphereEffect.HitColor;
        data.ActiveSecondaryColor = sphereEffect.SecondaryColor;
        data.ActiveHitDuration = sphereEffect.HitDuration;
        data.ActiveSecondaryRatio = sphereEffect.SecondaryColorRatio;

        data.LastHitSphere = sphereEntity;
        
        // 計時器現在要吃球給的秒數
        data.CurrentTimer = data.ActiveHitDuration; 
        data.HasChangedToSecondary = false; 
        
        DataLookup[goofyEntity] = data; 

        // 塗上這顆球指定的第一層顏色
        ColorLookup[data.SaberLeft] = new EmissionColor { Value = data.ActiveHitColor };
        ColorLookup[data.SaberRight] = new EmissionColor { Value = data.ActiveHitColor };
    }
}

// ==========================================
// 變色倒數與恢復系統
// ==========================================

[BurstCompile]
public partial struct GoofyTimerJob : IJobEntity
{
    public float DeltaTime;
    
    // 允許並行寫入顏色
    [NativeDisableParallelForRestriction] 
    public ComponentLookup<EmissionColor> ColorLookup;

    public void Execute(ref GoofyAssData data)
    {
        if (data.CurrentTimer > 0f)
        {
            data.CurrentTimer -= DeltaTime;

            // 👇 修改 1：換成吃球傳過來的 ActiveHitDuration 和 ActiveSecondaryRatio
            if (!data.HasChangedToSecondary && data.CurrentTimer <= data.ActiveHitDuration * data.ActiveSecondaryRatio)
            {
                data.HasChangedToSecondary = true; 
                
                // 👇 修改 2：換成塗上球傳過來的 ActiveSecondaryColor
                ColorLookup[data.SaberLeft] = new EmissionColor { Value = data.ActiveSecondaryColor };
                ColorLookup[data.SaberRight] = new EmissionColor { Value = data.ActiveSecondaryColor };
            }

            if (data.CurrentTimer <= 0f)
            {
                data.CurrentTimer = 0f; 
                data.HasChangedToSecondary = false; 
                
                // (維持不變) 倒數結束，塗回觀眾自己一開始抽到的預設顏色
                ColorLookup[data.SaberLeft] = new EmissionColor { Value = data.OriginalColor };
                ColorLookup[data.SaberRight] = new EmissionColor { Value = data.OriginalColor };
            }
        }
    }
}

[BurstCompile]
public partial struct SphereGrowthSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // 尋找世界上所有同時擁有 LocalTransform (可讀寫) 和 GrowthRateData (唯讀) 的實體
        foreach (var (transform, growthRate) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<GrowthRateData>>())
        {
            // 將球的 Scale 加上 (膨脹速度 * 經過時間)
            transform.ValueRW.Scale += growthRate.ValueRO.Value * deltaTime;
        }
    }
}


// ==========================================
// 觀眾出生時的顏色初始化系統
// ==========================================
[BurstCompile]

public partial struct GoofyColorInitSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // 尋找所有帶有 GoofyAssData，且【還沒有】 ColorInitializedTag 的觀眾
        foreach (var (data, entity) in SystemAPI.Query<RefRO<GoofyAssData>>().WithNone<ColorInitializedTag>().WithEntityAccess())
        {
            // 👇 關鍵修正：直接用 SystemAPI.SetComponent 強制修改光劍身上的顏色！
            SystemAPI.SetComponent(data.ValueRO.SaberLeft, new EmissionColor { Value = data.ValueRO.OriginalColor });
            SystemAPI.SetComponent(data.ValueRO.SaberRight, new EmissionColor { Value = data.ValueRO.OriginalColor });

            // 在待辦清單裡寫下：給這個母體貼上「已初始化」的貼紙，下次就不會再進來了
            ecb.AddComponent<ColorInitializedTag>(entity);
        }

        // 統一執行清單 (只負責貼貼紙)
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}