using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualButton : MonoBehaviour
{
    public Color OriginalColor;
    public Color TriggerColor;
    public Color StayOnColor;
    public bool StayOn;
    public OVRInput.Button TriggerButton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Controllor" )
        {
            if(StayOn)
                gameObject.GetComponent<Renderer>().material.color = StayOnColor;
            else
                gameObject.GetComponent<Renderer>().material.color = TriggerColor;
                
            if(OVRInput.GetDown(TriggerButton))
            {
                StayOn = !StayOn;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Controllor" && !StayOn)
        {
            gameObject.GetComponent<Renderer>().material.color = OriginalColor;
        }
    }
}
