using Unity.Entities;
using Unity.Mathematics;

// 儲存每一幀的位移與旋轉資料
public struct TransformFrame
{
    public float3 Position;
    public quaternion Rotation;
}

// ECS 專用的二進位動畫資料庫 (Blob Asset)
public struct AnimationBlob
{
    public float ClipLength;  // 動畫總長度 (秒)
    public int FrameCount;    // 總幀數
    public BlobArray<TransformFrame> Frames; // 存放所有幀的陣列
}

// 掛在實體上的組件，用來控制播放進度
public struct ECSAnimatorComponent : IComponentData
{
    public BlobAssetReference<AnimationBlob> Blob;
    public float CurrentTime;
    public float PlaybackSpeed;
}