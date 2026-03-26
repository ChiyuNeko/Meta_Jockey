using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GoofyColorAuthoring : MonoBehaviour
{
    public GameObject leftSaber; 
    public GameObject rightSaber; 

    // 母物件只要設定撞到會變什麼顏色就好
    [ColorUsage(true, true)] public Color hitColor = Color.red;       

    class Baker : Baker<GoofyColorAuthoring>
    {
        public override void Bake(GoofyColorAuthoring authoring)
        {
            if (authoring.leftSaber == null || authoring.rightSaber == null) return;

            Entity parentEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            // 取得子物件的 ID (這是合法的，我們只是「看」它們是誰，沒有要修改它們)
            Entity saberL = GetEntity(authoring.leftSaber, TransformUsageFlags.Dynamic);
            Entity saberR = GetEntity(authoring.rightSaber, TransformUsageFlags.Dynamic);

            float4 hColor = new float4(authoring.hitColor.r, authoring.hitColor.g, authoring.hitColor.b, authoring.hitColor.a);

            // 只在母物件自己身上加 Component
            AddComponent<GoofyAssTag>(parentEntity);
            AddComponent(parentEntity, new GoofyAssData
            {
                SaberLeft = saberL,
                SaberRight = saberR,
                HitColor = hColor
            });
        }
    }
}