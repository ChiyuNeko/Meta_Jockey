using UnityEngine;

public class WarpController : MonoBehaviour
{
    public SpaceObjectManager objectManager;
    public CameraWarpEffect cameraEffect;

    bool isWarp;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isWarp = !isWarp;
            objectManager.SetWarpMode(isWarp);
            cameraEffect.SetWarp(isWarp);
        }
    }
}

