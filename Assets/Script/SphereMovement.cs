using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    public Vector3 speed;
    public Vector3 Range;
    public float smooth;
    public Vector3 Seed;
    public bool RandomSeed;
    Vector3 offset;
    Vector3 gameObjectPosition;
    float Offset;
    void Start()
    {
        if(RandomSeed)
        {
            Seed.x = Random.Range(0,360);
            Seed.y = Random.Range(0,360);
            Seed.z = Random.Range(0,360);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Floating();
    }
    
    public void Floating()
    {
        Vector3 Destination;
        
        offset.x += speed.x;
        offset.y += speed.y;
        offset.z += speed.z;

        if(offset.x > 360)
            offset.x = 0;
        if(offset.y > 360)
            offset.y = 0;
        if(offset.z > 360)
            offset.z = 0;

        gameObjectPosition.x = Range.x * Mathf.Sin(offset.x + Seed.x) * Time.deltaTime;
        gameObjectPosition.y = Range.y * Mathf.Sin(offset.y + Seed.y) * Time.deltaTime;
        gameObjectPosition.z = Range.z * Mathf.Sin(offset.z + Seed.z) * Time.deltaTime;

        Destination = gameObject.transform.position + gameObjectPosition;
        gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, Destination, 1f/smooth);
    }
}
