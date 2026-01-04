using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float moveScale;
    
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for Follow script on " + gameObject.name);
        }
    }
    void Update()
    {
        if (target != null)
        {
            transform.localPosition = target.localPosition * moveScale + offset;
           // transform.rotation = target.rotation;            
        }

    }
}
