using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;
using UnityEngine.Audio;
using Unity.Mathematics;

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
    public AudioSource[] LoopSet1;
    public AudioSource[] LoopSet2;
    public AudioSource[] audioSource;
    public AudioMixer audioMixer;
    public float EQ;
    public float Vol;
    public ArduinoData arduinoData;
    public ParticleSystem particleSystems; 
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    bool flag5 = false;
    bool flag6 = false;
    bool flag7 = false;
    [Header("Controller Parameters")]
    public List<GameObject> Buttons = new List<GameObject>();
    public Material OriginalColor;
    public Material TriggerColor;
    public float RecoverTime;
    public int LoopCount;
    public GameObject[] BigCircle;
    public float BigCircle1Rot;
    public float BigCircle2Rot;
    void Start()
    {
        ArduinoBasic arduinoBasic = new ArduinoBasic();
    }

    // Update is called once per frame
    void Update()
    {
        switch (LoopCount)
        {
            case 0:
                if (BigCircle[0].activeSelf)
                {
                    foreach (GameObject i in BigCircle)
                    {
                        i.SetActive(false);
                    }
                }
                break;
            case 1:
                if(!BigCircle[0].activeSelf)
                    BigCircle[0].SetActive(true);
                if(BigCircle[1].activeSelf)
                    BigCircle[1].SetActive(false);
                break;

            case 2:
                if(!BigCircle[1].activeSelf)
                    BigCircle[1].SetActive(true);
                if(BigCircle[2].activeSelf)
                    BigCircle[2].SetActive(false);
                break;

            case 3:
                if(!BigCircle[2].activeSelf)
                    BigCircle[2].SetActive(true);
                break;

            default:
                break;
        }
        BigCircle1Rot = Mathf.Lerp(BigCircle1Rot, arduinoData.encoder, 0.01f);
        BigCircle[0].transform.rotation = quaternion.Euler(0, 90, BigCircle1Rot);
        BigCircle[1].transform.Rotate(0, 0, (arduinoData.encoder2 + 20) * Time.deltaTime * 3);

        EQ = Mathf.Lerp(EQ, arduinoData.encoder * 500 + 10000, 0.1f) ;
        EQ = Mathf.Clamp(EQ, 100, 10000);
        audioMixer.SetFloat("BaseLowPass", EQ);
        
        Vol = Mathf.Lerp(Vol, arduinoData.encoder2 * 4, 0.1f) ;
        Vol = Mathf.Clamp(Vol, -80, 10);
        audioMixer.SetFloat("Vol", Vol);
        
        if (Input.GetKeyDown(KeyCode.Keypad1) || (arduinoData.button1 == 0 && flag1 == false))
        {
            Buttons[0].GetComponent<Renderer>().material = TriggerColor;
            audioSource[0].Play();
            StartCoroutine(ButtonRecover(Buttons[0], RecoverTime));
            particleSystems.Play();
            flag1 = true;
        }
        if (arduinoData.button1 == 1)
        {
            flag1 = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2) || (arduinoData.button2 == 0 && flag2 == false))
        {
            Buttons[1].GetComponent<Renderer>().material = TriggerColor;
            audioSource[1].Play();
            StartCoroutine(ButtonRecover(Buttons[1], RecoverTime));
            particleSystems.Play();
            flag2 = true;
        }
        if (arduinoData.button2 == 1)
        {
            flag2 = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3) || (arduinoData.button3 == 0 && flag3 == false))
        {
            Buttons[2].GetComponent<Renderer>().material = TriggerColor;
            audioSource[2].Play();
            StartCoroutine(ButtonRecover(Buttons[2], RecoverTime));
            particleSystems.Play();
            flag3 = true;
        }
        if (arduinoData.button3 == 1)
        {
            flag3 = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4) || (arduinoData.button4 == 0 && flag4 == false))
        {
            Buttons[3].GetComponent<Renderer>().material = TriggerColor;
            audioSource[3].Play();
            StartCoroutine(ButtonRecover(Buttons[3], RecoverTime));
            flag4 = true;
        }
        if (arduinoData.button4 == 1)
        {
            flag4 = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad5) || (arduinoData.button5 == 0 && flag5 == false))
        {
            Buttons[4].GetComponent<Renderer>().material = TriggerColor;
            if (!audioSource[4].gameObject.activeSelf)
            {
                audioSource[4].gameObject.SetActive(true);
                audioSource[4].Play();
                LoopCount++;
            }
            else
            {
                audioSource[4].gameObject.SetActive(false);
                LoopCount--;
            }
            StartCoroutine(ButtonRecover(Buttons[4], RecoverTime));
            flag5 = true;
        }
        if (arduinoData.button5 == 1)
        {
            flag5 = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad6) || (arduinoData.button6 == 0 && flag6 == false))
        {
            Buttons[5].GetComponent<Renderer>().material = TriggerColor;
            if (!audioSource[5].gameObject.activeSelf)
            {
                audioSource[5].gameObject.SetActive(true);
                audioSource[5].Play();
                LoopCount++;
            }
            else
            {
                audioSource[5].gameObject.SetActive(false);
                LoopCount--;
            }
            StartCoroutine(ButtonRecover(Buttons[5], RecoverTime));
            flag6 = true;
        }
        if (arduinoData.button6== 1)
        {
            flag6 = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad7) || (arduinoData.button7 == 0 && flag7 == false))
        {
            Buttons[6].GetComponent<Renderer>().material = TriggerColor;
            if (!audioSource[6].gameObject.activeSelf)
            {
                audioSource[6].gameObject.SetActive(true);
                audioSource[6].Play();
                LoopCount++;
            }
            else
            {
                audioSource[6].gameObject.SetActive(false);
                LoopCount--;
            }
            StartCoroutine(ButtonRecover(Buttons[6], RecoverTime));
            flag7 = true;
        }
        if (arduinoData.button7 == 1)
        {
            flag7 = false;
        }

        if (Input.GetKey(KeyCode.Space))
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
