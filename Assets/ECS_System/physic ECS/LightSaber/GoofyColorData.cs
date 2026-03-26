using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

// 控制 Shader 發光的變數
[MaterialProperty("_EmissionColor")]
public struct EmissionColor : IComponentData
{
    public float4 Value;
}

// 掛在 GoofyAss 身上的大腦，記住兩根光劍
public struct GoofyAssData : IComponentData
{
    public Entity SaberLeft;
    public Entity SaberRight;
    public float4 HitColor; // 被撞到要變成什麼顏色
}

// 標籤
public struct GoofyAssTag : IComponentData {}
public struct TriggerSphereTag : IComponentData {}