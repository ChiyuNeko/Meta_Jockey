using UnityEngine;
using System.Collections.Generic;

public class SemiCircleWave : MonoBehaviour
{
    public BPMSpawner bpmSpawner;
    [Header("Blocks")]
    public GameObject blockPrefab;
    public int blockCount = 20;
    public float radius = 5f;
    public float spacing;

    [Header("Spawn Center")]
    public Transform spawnCenter;

    [Header("Materials")]
    public Material[] materials;

    public enum MaterialMode
    {
        Sequential,
        Random,
        CenterHighlight
    }

    public MaterialMode materialMode = MaterialMode.Sequential;

    [Header("Motion")]
    public float bpm = 120f;
    public float height = 1.5f;
    public float phaseOffset = 0.4f;

    private List<Transform> blocks = new List<Transform>();
    private float beatSpeed;

    void Start()
    {
        bpm = bpmSpawner.bpm;
        beatSpeed = 0.5f*(bpm / 60f) * Mathf.PI * 2f;
        CreateBlocks();
    }

    void CreateBlocks()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        blocks.Clear();

        Vector3 center = spawnCenter ? spawnCenter.position : transform.position;

        for (int i = 0; i < blockCount; i++)
        {
            float t = 1f / (blockCount);
            float angle = t * i * 360 * Mathf.Deg2Rad;
            Debug.Log(angle);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );

            GameObject block = Instantiate(
                blockPrefab,
                center + offset,
                Quaternion.identity,
                transform
            );

            ApplyMaterial(block, i);

            blocks.Add(block.transform);
        }
    }

    void ApplyMaterial(GameObject block, int index)
    {
        if (materials == null || materials.Length == 0) return;

        Renderer r = block.GetComponent<Renderer>();
        if (!r) return;

        switch (materialMode)
        {
            case MaterialMode.Sequential:
                r.material = materials[index % materials.Length];
                break;

            case MaterialMode.Random:
                r.material = materials[Random.Range(0, materials.Length)];
                break;

            case MaterialMode.CenterHighlight:
                int center = blockCount / 2;
                int dist = Mathf.Abs(index - center);
                int matIndex = Mathf.Clamp(dist, 0, materials.Length - 1);
                r.material = materials[matIndex];
                break;
        }
    }

    void Update()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            float wave = Mathf.Sin(Time.time * beatSpeed + i * phaseOffset);
            Vector3 p = blocks[i].localPosition;
            p.y = wave * height;
            blocks[i].localPosition = p;
        }
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            beatSpeed = (bpm / 60f) * Mathf.PI * 2f;
    }
}
