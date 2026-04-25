using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HandInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenGunPose(VisualEffect vfx)
    {
        vfx.SetBool("CanSpawn", true);
    }
    public void CloseGunPose(VisualEffect vfx)
    {
        vfx.SetBool("CanSpawn", false);
    }
}
