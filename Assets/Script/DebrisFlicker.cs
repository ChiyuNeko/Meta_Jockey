using UnityEngine;

public class DebrisFlicker : MonoBehaviour
{
    public Renderer targetRenderer;
    public float flickerChance = 0.005f;   // 越小越稀有
    public float flickerStrength = 1.5f;   // 亮度倍數
    public float flickerDuration = 0.05f;

    Color originalColor;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        originalColor = targetRenderer.material.color;
    }

    void Update()
    {
        if (Random.value < flickerChance)
        {
            StopAllCoroutines();
            StartCoroutine(Flicker());
        }
    }

    System.Collections.IEnumerator Flicker()
    {
        targetRenderer.material.color = originalColor * flickerStrength;
        yield return new WaitForSeconds(flickerDuration);
        targetRenderer.material.color = originalColor;
    }
}

