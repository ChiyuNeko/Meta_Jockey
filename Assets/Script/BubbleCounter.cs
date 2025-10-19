using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCounter : MonoBehaviour
{
    public int BubbleCount = 0;
    public GameObject TravleButton;
    public GameObject travelVFXPos;
    public GameObject travelVFX;
    public static BubbleCounter bubbleCounter = new BubbleCounter();
    void Start()
    {
        bubbleCounter = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (BubbleCount >= 20)
        {
            TravleButton.SetActive(true);
            BubbleCount = 0;
        }
    }

    public void TravelVFX()
    {
        Instantiate(travelVFX, travelVFXPos.transform.position, Quaternion.Euler(0, -90, 0), travelVFXPos.transform);
    }
}
