using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_Manerger : MonoBehaviour
{
    public AudioSource clap;
    

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            clap.play;
        }
    }
}
