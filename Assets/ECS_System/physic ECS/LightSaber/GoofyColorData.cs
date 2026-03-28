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
    public float4 OriginalColor;     
    public float CurrentTimer;
    public Entity LastHitSphere;
    public bool HasChangedToSecondary; 
    
    // 👇 新增：用來「暫存」撞到他的那顆球所傳遞過來的特效資料
    public float4 ActiveHitColor;          
    public float4 ActiveSecondaryColor; 
    public float ActiveHitDuration;
    public float ActiveSecondaryRatio; 
}

// 標籤
public struct GoofyAssTag : IComponentData {}
public struct TriggerSphereTag : IComponentData {}
public struct ColorInitializedTag : IComponentData {}