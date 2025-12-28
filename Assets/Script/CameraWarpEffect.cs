using UnityEngine;

public class CameraWarpEffect : MonoBehaviour
{
    public float normalFOV = 60f;
    public float warpFOV = 90f;
    public float shakeIntensity = 0.3f;

    Vector3 originalPos;
    Camera cam;
    bool isWarp;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalPos = transform.localPosition;
    }

    void Update()
    {
        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            isWarp ? warpFOV : normalFOV,
            Time.deltaTime * 3f
        );

        if (isWarp)
        {
            transform.localPosition =
                originalPos + Random.insideUnitSphere * shakeIntensity;
        }
        else
        {
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * 5f);
        }
    }

    public void SetWarp(bool warp)
    {
        isWarp = warp;
    }
}
