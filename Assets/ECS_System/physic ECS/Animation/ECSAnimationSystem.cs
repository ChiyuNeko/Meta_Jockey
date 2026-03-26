using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct ECSAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 啟動多執行緒 Job
        var job = new ECSAnimationJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ECSAnimationJob : IJobEntity
{
    public float DeltaTime;

    // 自動找出所有帶有 LocalTransform 和我們自訂動畫資料的實體
    public void Execute(ref LocalTransform transform, ref ECSAnimatorComponent animData)
    {
        if (!animData.Blob.IsCreated) return;

        ref var blob = ref animData.Blob.Value;
        
        // 推進時間並計算 Loop
        animData.CurrentTime += DeltaTime * animData.PlaybackSpeed;
        if (animData.CurrentTime >= blob.ClipLength)
        {
            animData.CurrentTime %= blob.ClipLength;
        }

        // 計算目前處於哪兩幀之間
        float frameRatio = animData.CurrentTime / blob.ClipLength;
        float exactFrame = frameRatio * (blob.FrameCount - 1);
        
        int frameIndex1 = (int)math.floor(exactFrame);
        int frameIndex2 = (int)math.ceil(exactFrame);
        float lerpFactor = exactFrame - frameIndex1;

        // 防呆保護
        frameIndex1 = math.clamp(frameIndex1, 0, blob.FrameCount - 1);
        frameIndex2 = math.clamp(frameIndex2, 0, blob.FrameCount - 1);

        // 取得兩幀的資料
        TransformFrame frame1 = blob.Frames[frameIndex1];
        TransformFrame frame2 = blob.Frames[frameIndex2];

        // 進行線性差值 (Lerp/Slerp)，讓動畫滑順度超越原本的幀率
        transform.Position = math.lerp(frame1.Position, frame2.Position, lerpFactor);
        transform.Rotation = math.slerp(frame1.Rotation, frame2.Rotation, lerpFactor);
    }
}