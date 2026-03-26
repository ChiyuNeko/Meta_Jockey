using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SaberColorAuthoring : MonoBehaviour
{
    [ColorUsage(true, true)] public Color defaultColor = Color.white;

    class Baker : Baker<SaberColorAuthoring>
    {
        public override void Bake(SaberColorAuthoring authoring)
        {
            // 取得自己 (光劍) 的 Entity
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            float4 defColor = new float4(authoring.defaultColor.r, authoring.defaultColor.g, authoring.defaultColor.b, authoring.defaultColor.a);
            
            // 自己幫自己加上發光顏色 Component
            AddComponent(entity, new EmissionColor { Value = defColor });
        }
    }
}