using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

// ⚠️ 注意：這次這個腳本要掛在「子物件 (螢光棒)」上！
public class ECSAnimatorAuthoring : MonoBehaviour
{
    [Header("帶有 Animator 的父物件 (用來讀取動畫路徑)")]
    public GameObject animatorRoot; 

    [Header("你要播放的 Animation Clip")]
    public AnimationClip clip; 
    
    public float sampleFrameRate = 30f;
    public float playbackSpeed = 1f;

    class Baker : Baker<ECSAnimatorAuthoring>
    {
        public override void Bake(ECSAnimatorAuthoring authoring)
        {
            if (authoring.clip == null || authoring.animatorRoot == null) return;

            // 因為腳本掛在子物件上，GetEntity 拿到的就是「子物件自己的 Entity」！合法！
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            var builder = new Unity.Entities.BlobBuilder(Unity.Collections.Allocator.Temp);
            ref var root = ref builder.ConstructRoot<AnimationBlob>();

            root.ClipLength = authoring.clip.length;
            root.FrameCount = Mathf.CeilToInt(authoring.clip.length * authoring.sampleFrameRate);
            var frameArray = builder.Allocate(ref root.Frames, root.FrameCount);

            // 備份「自己(子物件)」原本的 Transform
            Vector3 originalPos = authoring.transform.localPosition;
            Quaternion originalRot = authoring.transform.localRotation;

            for (int i = 0; i < root.FrameCount; i++)
            {
                float time = (i / (float)root.FrameCount) * authoring.clip.length;
                
                // 【核心魔法】對「父物件 (animatorRoot)」進行取樣
                // Unity 底層會自動計算動畫曲線，並把結果套用到子物件上
                authoring.clip.SampleAnimation(authoring.animatorRoot, time);

                // 抄寫下此時「自己(子物件)」的 XYZ 和旋轉
                frameArray[i] = new TransformFrame
                {
                    Position = authoring.transform.localPosition,
                    Rotation = authoring.transform.localRotation
                };
            }

            // 恢復原本的 Transform
            authoring.transform.localPosition = originalPos;
            authoring.transform.localRotation = originalRot;

            var blobAsset = builder.CreateBlobAssetReference<AnimationBlob>(Unity.Collections.Allocator.Persistent);
            AddBlobAsset(ref blobAsset, out var hash); 

            // 合法地把資料加給自己
            AddComponent(entity, new ECSAnimatorComponent
            {
                Blob = blobAsset,
                CurrentTime = UnityEngine.Random.Range(0f, authoring.clip.length), 
                PlaybackSpeed = authoring.playbackSpeed
            });

            builder.Dispose();
        }
    }
}