using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotKey : MonoBehaviour
{
    // Start is called before the first frame update
    public Material GroundGrid;
    public Color GridColor1;
    public Color GridColor2;
    public GameObject neon;
    public Transform SpawnPoint;
    public float NeonSpace;
    public float NeonSpeed;
    public GameObject Camera;
    public float CameraSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GroundChangeColor(GridColor1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            GroundChangeColor(GridColor2);
        }
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            NeonLine(Vector3.left * NeonSpace , Vector3.down * NeonSpeed);
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            NeonLine(Vector3.zero , Vector3.down * NeonSpeed);
        }
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            NeonLine(Vector3.right * NeonSpace , Vector3.down * NeonSpeed);
        }
        if(Input.GetKeyDown(KeyCode.Keypad4))
        {
            NeonLine(new Vector3(-3, -1, 0) * NeonSpace , Vector3.right * NeonSpeed);
        }
        if(Input.GetKeyDown(KeyCode.Keypad5))
        {
            NeonLine(new Vector3(3, -2, 0) * NeonSpace , Vector3.left * NeonSpeed);
        }
        if(Input.GetKey(KeyCode.Space))
        {
            Camera.transform.Translate(Vector3.forward * CameraSpeed * Time.deltaTime);
        }
    }

    public void GroundChangeColor(Color color)
    {
        GroundGrid.SetColor("_BackgroundColor", color);
    }

    public void NeonLine(Vector3 PositionOffset ,Vector3 direct) 
    {
        GameObject gameObject = Instantiate(neon, SpawnPoint.position + PositionOffset, Quaternion.identity);
        gameObject.GetComponent<Rigidbody>().AddForce(direct);
        Destroy(gameObject, 3);
    }
}
