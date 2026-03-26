using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LightSaberAuthoring : MonoBehaviour
{
    public GameObject lightSaber1; 
    public GameObject lightSaber2; 

    // 改成陣列，這樣你在 Inspector 想放 3 種或 10 種隨機顏色都可以
    [ColorUsage(true, true)] public Color[] randomColors;
    [ColorUsage(true, true)] public Color hitColor;
    public float hitDuration = 2.0f;

    class Baker : Baker<LightSaberAuthoring>
    {
        public override void Bake(LightSaberAuthoring authoring)
        {
            if (authoring.lightSaber1 == null || authoring.lightSaber2 == null || authoring.randomColors.Length == 0) return;

            Entity saber1 = GetEntity(authoring.lightSaber1, TransformUsageFlags.Dynamic);
            Entity saber2 = GetEntity(authoring.lightSaber2, TransformUsageFlags.Dynamic);

            // 隨機抽色並轉換成 ECS 的 float4
            Color pickedColor = authoring.randomColors[UnityEngine.Random.Range(0, authoring.randomColors.Length)];
            float4 origColor = new float4(pickedColor.r, pickedColor.g, pickedColor.b, pickedColor.a);
            float4 hColor = new float4(authoring.hitColor.r, authoring.hitColor.g, authoring.hitColor.b, authoring.hitColor.a);

            var saberData = new LightSaberData
            {
                OriginalColor = origColor,
                HitColor = hColor,
                HitDuration = authoring.hitDuration,
                CurrentTimer = 0f 
            };

            // 賦予光劍 1 號資料與標籤
            AddComponent<LightSaberTag>(saber1);
            AddComponent(saber1, saberData);
            AddComponent(saber1, new EmissionColor { Value = origColor });

            // 賦予光劍 2 號資料與標籤
            AddComponent<LightSaberTag>(saber2);
            AddComponent(saber2, saberData);
            AddComponent(saber2, new EmissionColor { Value = origColor });
        }
    }
}