using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

// 負責驅動 URP Shader 發光的顏色資料
[MaterialProperty("_EmissionColor")]
public struct EmissionColor : IComponentData
{
    public float4 Value;
}

// 光劍的變色狀態與計時器
public struct LightSaberData : IComponentData
{
    public float4 OriginalColor;
    public float4 HitColor;
    public float HitDuration;
    public float CurrentTimer;
}

// 標記「我是光劍」
public struct LightSaberTag : IComponentData {}

// 標記「我是觸發器(擴散球)」- 收編進來，達到完全自給自足
public struct LightSaberTriggerTag : IComponentData {}