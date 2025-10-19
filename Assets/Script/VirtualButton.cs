using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VirtualButton : MonoBehaviour
{
    public Color OriginalColor;
    public Color TriggerColor;
    public Color StayOnColor;
    public bool StayOn;
    public OVRInput.Button TriggerButton;
    public AudioSource audioSource;
    public ParticleSystem particleSystem;
    public bool IsSet = false;
    public int SetNum;
    public bool trigger = false;
    public HotKey hotKey;
    public UnityEvent onTrigger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Controllor" && !IsSet)
        {
            if (StayOn)
            {
                gameObject.GetComponent<Renderer>().material.color = StayOnColor;
                audioSource.loop = true;
            }
            else
            {
                if(!trigger)
                {
                    gameObject.GetComponent<Renderer>().material.color = TriggerColor;
                    if (audioSource)
                        audioSource.Play();
                    if(particleSystem)
                        particleSystem.Play();
                    audioSource.loop = false;
                    onTrigger?.Invoke();
                    trigger = true;
                }
            }

            if (OVRInput.GetDown(TriggerButton))
            {
                StayOn = !StayOn;
            }
        }
        else if (other.tag == "Controllor" && IsSet)
        {
            gameObject.GetComponent<Renderer>().material.color = TriggerColor;
            particleSystem.Play();
            if (SetNum == 1)
            {
                hotKey.audioSource = hotKey.LoopSet1;
            }
            else if (SetNum == 2)
            {
                hotKey.audioSource = hotKey.LoopSet2;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Controllor" && !StayOn)
        {
            gameObject.GetComponent<Renderer>().material.color = OriginalColor;
            trigger = false;
        }
    }
}
