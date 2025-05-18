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
    [Header("Controller Parameters")]
    public List<GameObject> Buttons = new List<GameObject>();
    public Material OriginalColor;
    public Material TriggerColor;
    public float RecoverTime;
    public AudioSource audioSource;

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
            Buttons[0].GetComponent<Renderer>().material = TriggerColor;
            audioSource.Play();
            audioSource.pitch = 1;
            StartCoroutine(ButtonRecover(Buttons[0], RecoverTime));
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            NeonLine(Vector3.zero , Vector3.down * NeonSpeed);
            Buttons[1].GetComponent<Renderer>().material = TriggerColor;
            audioSource.Play();
            audioSource.pitch = 1.05f;
            StartCoroutine(ButtonRecover(Buttons[1], RecoverTime));
        }
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            NeonLine(Vector3.right * NeonSpace , Vector3.down * NeonSpeed);
            Buttons[2].GetComponent<Renderer>().material = TriggerColor;
            audioSource.Play();
            audioSource.pitch = 1.1f;
            StartCoroutine(ButtonRecover(Buttons[2], RecoverTime));
        }
        if(Input.GetKeyDown(KeyCode.Keypad4))
        {
            NeonLine(new Vector3(-3, -1, 0) * NeonSpace , Vector3.right * NeonSpeed);
            Buttons[3].GetComponent<Renderer>().material = TriggerColor;
            audioSource.Play();
            audioSource.pitch = 1.15f;
            StartCoroutine(ButtonRecover(Buttons[3], RecoverTime));
        }
        if(Input.GetKeyDown(KeyCode.Keypad5))
        {
            NeonLine(new Vector3(3, -2, 0) * NeonSpace , Vector3.left * NeonSpeed);
            Buttons[4].GetComponent<Renderer>().material = TriggerColor;
            audioSource.Play();
            audioSource.pitch = 1.2f;
            StartCoroutine(ButtonRecover(Buttons[4], RecoverTime));
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

    IEnumerator ButtonRecover(GameObject Button, float RecoverTime)
    {
        yield return new WaitForSeconds(RecoverTime);

        string nowMaterial =  Button.GetComponent<Renderer>().material.name;
        Debug.Log("" + nowMaterial+TriggerColor.name);

        if(nowMaterial == TriggerColor.name + " (Instance)")
        {
            Button.GetComponent<Renderer>().material = OriginalColor;
        }
    
    }
}
