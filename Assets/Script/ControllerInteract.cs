using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInteract : MonoBehaviour
{
    public GameObject controller;
    public GameObject prefab;
    public GameObject aim;
    public float force;
    Rigidbody rigidbody;

    void Update()
    {
        follow();
    }
    public void follow()
    {
        gameObject.transform.position = controller.transform.position;
        gameObject.transform.rotation = controller.transform.rotation;
    }
    public void Shoot()
    {
        GameObject g = Instantiate(prefab, aim.transform.position, controller.transform.rotation);
        if(g.GetComponent<Rigidbody>())
            rigidbody = g.GetComponent<Rigidbody>();
        rigidbody.AddForce(g.transform.forward * force);
    }
}
