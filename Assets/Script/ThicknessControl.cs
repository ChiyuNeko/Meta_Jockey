using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

[ExecuteAlways]
public class ThicknessController : MonoBehaviour
{
    [Header("Effect 1")]
    public VisualEffect[] vfxs1;
    [Range(0f, 1f)]
    public float originalThickness1 = 1f;
    public KeyCode triggerKey1 = KeyCode.Space;

    [Header("Effect 2")]
    public VisualEffect[] vfxs2;
    [Range(0f, 1f)]
    public float originalThickness2 = 1f;
    public KeyCode triggerKey2 = KeyCode.Return;

    public string thicknessProperty = "thickness";
    public float fadeDuration = 1f;

    private bool isFading1 = false;
    private bool isAtZero1 = false;

    private bool isFading2 = false;
    private bool isAtZero2 = false;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey1) && !isFading1)
        {
            float from = isAtZero1 ? 0f : originalThickness1;
            float to = isAtZero1 ? originalThickness1 : 0f;

            StartCoroutine(FadeThickness(vfxs1, from, to, fadeDuration, () =>
            {
                isAtZero1 = !isAtZero1;
                isFading1 = false;
            }));

            isFading1 = true;
        }
        
        if (Input.GetKeyDown(triggerKey2) && !isFading2)
        {
            float from = isAtZero2 ? 0f : originalThickness2;
            float to = isAtZero2 ? originalThickness2 : 0f;

            StartCoroutine(FadeThickness(vfxs2, from, to, fadeDuration, () =>
            {
                isAtZero2 = !isAtZero2;
                isFading2 = false;
            }));

            isFading2 = true;
        }
    }
  
    private void SetAllThickness(VisualEffect[] vfxs, float thickness)
    {
        foreach (var vfx in vfxs)
        {
            if (vfx != null)
                vfx.SetFloat(thicknessProperty, thickness);
        }
    }
  
    private IEnumerator FadeThickness(VisualEffect[] vfxs, float from, float to, float duration, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float interpolated = Mathf.Lerp(from, to, t);
            SetAllThickness(vfxs, interpolated);
            yield return null;
        }
        SetAllThickness(vfxs, to);
        onComplete?.Invoke();
    }
}




