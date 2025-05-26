using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AnimationSlot
{
    public string animationState;
    public KeyCode triggerKey;
}

public class CrowdSpawner : MonoBehaviour
{
    [Header("Crowd Settings")]
    public List<GameObject> crowdPrefabs;
    public int rows = 10;
    public int columns = 10;
    public float spacing = 2.0f;
    public float randomOffset = 1.0f;

    [Header("Animation Slots")]
    public List<AnimationSlot> animationSlots = new List<AnimationSlot>();

    private List<GameObject> spawnedCrowd = new List<GameObject>();

    void Start()
    {
        SpawnCrowd();
    }

    void Update()
    {
        foreach (var slot in animationSlots)
        {
            if (Input.GetKeyDown(slot.triggerKey))
            {
                SetAnimationState(slot.animationState);
                Debug.Log($"Switched to animation: {slot.animationState} with key: {slot.triggerKey}");
            }
        }
    }

    void SpawnCrowd()
    {
        ClearCrowd();

        if (crowdPrefabs.Count == 0) 
        {
            Debug.LogError("No prefabs assigned to spawn!");
            return;
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(col * spacing, 0, row * spacing);

                // 增加隨機偏移來讓位置更隨機化
                position.x += Random.Range(-randomOffset, randomOffset);
                position.z += Random.Range(-randomOffset, randomOffset);

                GameObject prefab = crowdPrefabs[Random.Range(0, crowdPrefabs.Count)];
                GameObject crowdMember = Instantiate(prefab, position, Quaternion.identity, transform);

                Animator animator = crowdMember.GetComponent<Animator>();
                if (animator != null && animationSlots.Count > 0)
                {
                    animator.Play(animationSlots[0].animationState);
                }

                spawnedCrowd.Add(crowdMember);
            }
        }
    }

    void ClearCrowd()
    {
        foreach (var member in spawnedCrowd)
        {
            DestroyImmediate(member);
        }
        spawnedCrowd.Clear();
    }

    public void SetAnimationState(string stateName)
    {
        foreach (var member in spawnedCrowd)
        {
            Animator animator = member.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(stateName);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(CrowdSpawner))]
    public class CrowdSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CrowdSpawner spawner = (CrowdSpawner)target;
            if (GUILayout.Button("Preview Crowd"))
            {
                spawner.SpawnCrowd();
            }

            if (GUILayout.Button("Clear Crowd"))
            {
                spawner.ClearCrowd();
            }
        }
    }
    #endif
}

