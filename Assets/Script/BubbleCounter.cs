using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public class BubbleCounter : MonoBehaviour
{
    public SkyboxSwitcher skyboxSwitcher;
    public float BubbleCount;
    public float _bubbleCount{get; set;}
    public GameObject Sun;
    public float sunScale;
    public GameObject TravleButton;
    public GameObject travelVFXPos;
    public GameObject travelVFX;
    public GameObject Circle1;
    public GameObject Circle2;
    public Material Wall;
    public Material RingLight1;
    public Material RingLight2;
    public Material RingLight3;
    
    [ColorUsage(true, true)] 
    public Color RingLightColor1;
    [ColorUsage(true, true)] 
    public Color RingLightColor2;
    public float intnsitive1 = 0;
    public float intnsitive2 = 0;
    public float mainAlpha = 0;
    public float WallDissolve;
    public float _WallDissolve {get; set;}
    public float DissolveSpeed;
    public static BubbleCounter bubbleCounter = new BubbleCounter();
    public bool isTravaled;
    void Start()
    {
        bubbleCounter = this;
        BubbleCount = 0;
        Wall.SetFloat("_Dissolve", 80);
        RingLight1.SetColor("_EmissionColor", RingLightColor1 * 0);
        RingLight2.SetColor("_EmissionColor", RingLightColor2 * 0);
        RingLight3.SetFloat("_Main_Alpha", 0);
        WallDissolve = 80;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        BubbleCount = _bubbleCount * 40;
        BubbleCount = Mathf.Clamp(BubbleCount, 0, 40);
        Sun.transform.localScale = Vector3.Lerp(Sun.transform.localScale, Vector3.one * BubbleCount * sunScale, 0.1f);

        WallDissolve = Mathf.Lerp(WallDissolve, _WallDissolve * 80, Time.deltaTime/DissolveSpeed);
        Wall.SetFloat("_Dissolve", WallDissolve);

        //Sun.transform.localScale = Vector3.one * BubbleCount;
        if(BubbleCount < 10)
        {
            intnsitive1 = Mathf.Lerp(intnsitive1, 0, Time.deltaTime);
            RingLight1.SetColor("_EmissionColor", RingLightColor1 * intnsitive1);
            intnsitive2 = Mathf.Lerp(intnsitive2, 0, Time.deltaTime);
            mainAlpha = Mathf.Lerp(mainAlpha, 0, Time.deltaTime);
            RingLight2.SetColor("_EmissionColor", RingLightColor2 * intnsitive2);
            RingLight3.SetFloat("_Main_Alpha", mainAlpha);
        }
        if (BubbleCount >= 10 && BubbleCount < 20)
        {
            intnsitive1 = Mathf.Lerp(intnsitive1, 2, Time.deltaTime/2);
            Circle1.transform.Rotate(0, Time.deltaTime * 50, 0);
            RingLight1.SetColor("_EmissionColor", RingLightColor1 * intnsitive1);

            intnsitive2 = Mathf.Lerp(intnsitive2, 0, Time.deltaTime);
            mainAlpha = Mathf.Lerp(mainAlpha, 0, Time.deltaTime);
            RingLight2.SetColor("_EmissionColor", RingLightColor2 * intnsitive2);
            RingLight3.SetFloat("_Main_Alpha", mainAlpha);
        }
        if (BubbleCount >= 20)
        {
            intnsitive2 = Mathf.Lerp(intnsitive2, 4, Time.deltaTime/2);
            mainAlpha = Mathf.Lerp(mainAlpha, 1, Time.deltaTime/2);
            Circle1.transform.Rotate(0, Time.deltaTime * 50, 0);
            Circle2.transform.Rotate(0, -Time.deltaTime * 50, 0);
            RingLight2.SetColor("_EmissionColor", RingLightColor2 * intnsitive2);
            RingLight3.SetFloat("_Main_Alpha", mainAlpha);
        }
        if (BubbleCount >= 30)
        {
            
        }
        if (BubbleCount == 40)
        {
           // TravleButton.SetActive(true);
            if(!isTravaled)
            {
                Instantiate(travelVFX, travelVFXPos.transform.position, Quaternion.Euler(0, -90, 0), travelVFXPos.transform);
                isTravaled = true;
                skyboxSwitcher.TriggerSkyboxSwitch();
            }
        }
        if(BubbleCount < 40)
        {
            isTravaled = false;
        }
    }

    public void TravelVFX()
    {
        Instantiate(travelVFX, travelVFXPos.transform.position, Quaternion.Euler(0, -90, 0), travelVFXPos.transform);
        //TravleButton.SetActive(false);
    }
    
}
