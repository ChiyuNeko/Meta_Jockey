using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GoofyColorAuthoring : MonoBehaviour
{
    public GameObject leftSaber; 
    public GameObject rightSaber; 

    [Tooltip("觀眾平時待機的預設顏色")]
    [ColorUsage(true, true)] public Color defaultColor = Color.white; 
    
    // ⚠️ 注意：這裡我們已經把擴散波的外環、內環顏色、時間、比例全刪了！
    // 因為現在這些資料是由「撞到它的球 (Sphere)」來決定的。

    class Baker : Baker<GoofyColorAuthoring>
    {
        public override void Bake(GoofyColorAuthoring authoring)
        {
            if (authoring.leftSaber == null || authoring.rightSaber == null) return;

            Entity parentEntity = GetEntity(TransformUsageFlags.Dynamic);
            Entity saberL = GetEntity(authoring.leftSaber, TransformUsageFlags.Dynamic);
            Entity saberR = GetEntity(authoring.rightSaber, TransformUsageFlags.Dynamic);

            float4 defColor = new float4(authoring.defaultColor.r, authoring.defaultColor.g, authoring.defaultColor.b, authoring.defaultColor.a);

            AddComponent<GoofyAssTag>(parentEntity);
            AddComponent(parentEntity, new GoofyAssData
            {
                SaberLeft = saberL,
                SaberRight = saberR,
                OriginalColor = defColor,          // 觀眾唯一需要記住的：自己原本的顏色
                CurrentTimer = 0f,                  
                HasChangedToSecondary = false,
                LastHitSphere = Entity.Null,       // 一開始沒有被任何球撞過，填入空實體
                
                // 👇 這些是準備用來「接收」球傳遞過來的變數，一開始先塞 0 或空值即可
                ActiveHitColor = new float4(0, 0, 0, 0),
                ActiveSecondaryColor = new float4(0, 0, 0, 0),
                ActiveHitDuration = 0f,
                ActiveSecondaryRatio = 0f
            });
        }
    }
}