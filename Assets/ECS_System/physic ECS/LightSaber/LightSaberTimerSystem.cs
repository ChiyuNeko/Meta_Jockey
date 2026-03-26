using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct LightSaberTimerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new SaberTimerJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct SaberTimerJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref LightSaberData data, ref EmissionColor color)
    {
        if (data.CurrentTimer > 0f)
        {
            data.CurrentTimer -= DeltaTime;

            if (data.CurrentTimer <= 0f)
            {
                data.CurrentTimer = 0f; 
                color.Value = data.OriginalColor; 
            }
        }
    }
}