using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 moveScale;
    public bool followRotation;
    
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
            transform.localPosition =new Vector3(target.localPosition.x * moveScale.x + offset.x,
                                                target.localPosition.y * moveScale.y + offset.y, 
                                                target.localPosition.z * moveScale.z + offset.z) ;
            if (followRotation)
                transform.rotation = target.rotation;
        }

    }
}
