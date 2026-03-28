using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class TriggerSphereAuthoring : MonoBehaviour
{
    [Header("自毀與擴散設定")]
    public float lifeTime = 3.0f; 
    public float growthSpeed = 5.0f; 

    // 👇 就是這裡！這四個變數會出現在球的 Inspector 裡讓你調顏色！
    [Header("這顆球專屬的【顏料設定】")]
    [Tooltip("剛撞到時的邊緣顏色 (外環)")]
    [ColorUsage(true, true)] public Color hitColor = Color.cyan;       
    
    [Tooltip("經過比例時間後變成的顏色 (內環)")]
    [ColorUsage(true, true)] public Color secondaryColor = Color.blue; 
    
    [Tooltip("變成內環顏色的時機點。例如 0.85 代表外環極細")]
    [Range(0f, 1f)] public float secondaryColorRatio = 0.8f; 
    
    [Tooltip("變色後維持幾秒恢復原狀？")]
    public float hitDuration = 3.0f; 

    class Baker : Baker<TriggerSphereAuthoring>
    {
        public override void Bake(TriggerSphereAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent<TriggerSphereTag>(entity);
            AddComponent(entity, new LifeTimeData { Value = authoring.lifeTime });
            AddComponent(entity, new GrowthRateData { Value = authoring.growthSpeed });
            
            // 把 Inspector 裡調好的顏色，寫進這顆球的 ECS 記憶體中
            AddComponent(entity, new SphereEffectData
            {
                HitColor = new float4(authoring.hitColor.r, authoring.hitColor.g, authoring.hitColor.b, authoring.hitColor.a),
                SecondaryColor = new float4(authoring.secondaryColor.r, authoring.secondaryColor.g, authoring.secondaryColor.b, authoring.secondaryColor.a),
                SecondaryColorRatio = authoring.secondaryColorRatio,
                HitDuration = authoring.hitDuration
            });
        }
    }
}


public struct SphereEffectData : IComponentData
{
    public float4 HitColor;
    public float4 SecondaryColor;
    public float HitDuration;
    public float SecondaryColorRatio;
}

public struct LifeTimeData : IComponentData { public float Value; }