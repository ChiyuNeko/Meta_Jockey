using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor; // 引入編輯器專用的 API
#endif

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject spectatorPrefab;
    public int spawnCount = 1000;
    public float spawnRadius = 20f;
    public float scaleFactor = 1.0f; 
    public Vector3 rotationOffsetEuler = Vector3.zero;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            if (authoring.spectatorPrefab == null) return;

            Entity entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new SpawnerData
            {
                Prefab = GetEntity(authoring.spectatorPrefab, TransformUsageFlags.Dynamic),
                SpawnCount = authoring.spawnCount,
                SpawnRadius = authoring.spawnRadius,
                CenterPosition = authoring.transform.position,
                ScaleFactor = authoring.scaleFactor,
                BaseRotationOffset = quaternion.EulerZXY(
                    math.radians(authoring.rotationOffsetEuler.x), 
                    math.radians(authoring.rotationOffsetEuler.y), 
                    math.radians(authoring.rotationOffsetEuler.z)
                ),
                SpawnerRotation = authoring.transform.rotation 
            });
        }
    }

#if UNITY_EDITOR
    // 當你在 Scene 視窗中點選到這個物件時，就會觸發這個繪圖函式
    private void OnDrawGizmosSelected()
    {
        // 設定畫筆顏色為紅色
        Handles.color = Color.red;

        // 畫一個空心的紅色圓圈 (中心點, 圓盤的法線朝上, 半徑)
        Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);

        // (額外視覺優化) 畫一個超淡的紅色半透明圓盤，讓範圍更好辨識
        Handles.color = new Color(1f, 0f, 0f, 0.1f); 
        Handles.DrawSolidDisc(transform.position, Vector3.up, spawnRadius);
    }
#endif
}