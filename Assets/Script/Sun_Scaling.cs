using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun_Scaling : MonoBehaviour
{
    [Header("大小")]
    [Range(0f, 3f)] 
    public float Sun_Scale = 1f;

    void Update()
    {
        transform.localScale = Vector3.one * Sun_Scale;
    }
}