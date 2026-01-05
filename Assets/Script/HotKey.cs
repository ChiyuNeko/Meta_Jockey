using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;
using UnityEngine.Audio;
using Unity.Mathematics;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class HotKey : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] InputAction[] _noteActions = null;
    [SerializeField] InputAction _modWheelAction = null;
    static string[] NoteNames = new[] { "C", "C#", "D", "D#", "E", "F",
                                        "F#", "G", "G#", "A", "A#", "B" };
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
    public AudioSource mainMusic;
    public AudioMixer audioMixer;
    public float EQ;
    public float Vol;
    public ArduinoData arduinoData;
    public int inputindex;
    public ParticleSystem[] particleSystems;
    public VisualEffect visualEffect;
    [Header("Controller Parameters")]
    public List<GameObject> Buttons = new List<GameObject>();
    public Color OriginalColor;
    public Color TriggerColor;
    public float RecoverTime;
    public int LoopCount;
    public GameObject[] BigCircle;
    public float BigCircle1Rot;
    public float BigCircle2Rot;
    public GameObject laserEffect;
    public bool triggered = false;
    public Animator LightOn;
    public GameObject startButton;
    public BPMSpawner bPMSpawner;
    public float startProcess;
    public float activeSpeed;
    public float decreasSpeed;
    public SelfSpin selfSpin;
    void Start()
    {
        for (var i = 0; i < _noteActions.Length; i++) SetUpNoteAction(i);
        _modWheelAction.Enable();
        inputindex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        var mod = _modWheelAction.ReadValue<float>();
        Debug.Log("" + mod);

        startProcess -= decreasSpeed * Time.deltaTime;
        startProcess = Mathf.Clamp(startProcess, 0, 100);

        BigCircle1Rot = Mathf.Lerp(BigCircle1Rot, arduinoData.encoder, 0.01f);
        BigCircle[0].transform.rotation = quaternion.Euler(0, 90, BigCircle1Rot);
        BigCircle[1].transform.Rotate(0, 0, (arduinoData.encoder2 + 20) * Time.deltaTime * 3);

        EQ = Mathf.Lerp(EQ, (mod-0.2f) * 12500, 0.1f) ;
        EQ = Mathf.Clamp(EQ, 100, 10000);
        audioMixer.SetFloat("BaseLowPass", EQ);
        
        Vol = Mathf.Lerp(Vol, arduinoData.encoder2 * 4, 0.1f) ;
        Vol = Mathf.Clamp(Vol, -80, 10);
        audioMixer.SetFloat("Vol", Vol);
        
        if (Input.GetKeyDown(KeyCode.Alpha1) || inputindex == 0)
        {
            Buttons[0].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            audioSource[0].Play();
            StartCoroutine(ButtonRecover(Buttons[0], RecoverTime));
            visualEffect.Play();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || inputindex == 1)
        {
            Buttons[1].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            audioSource[1].Play();
            StartCoroutine(ButtonRecover(Buttons[1], RecoverTime));
            particleSystems[0].Play();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || inputindex == 2)
        {
            Buttons[2].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            audioSource[2].Play();
            StartCoroutine(ButtonRecover(Buttons[2], RecoverTime));
            visualEffect.Play();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || inputindex == 3)
        {
            Buttons[3].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            audioSource[3].Play();
            StartCoroutine(ButtonRecover(Buttons[3], RecoverTime));
            particleSystems[0].Play();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) || inputindex == 4)
        {
            Buttons[4].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                audioSource[4].gameObject.SetActive(!audioSource[4].gameObject.activeSelf);
                triggered = true;
            }
            StartCoroutine(ButtonRecover(Buttons[4], RecoverTime));
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) || inputindex == 5)
        {
            Buttons[5].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                audioSource[5].gameObject.SetActive(!audioSource[5].gameObject.activeSelf);
                triggered = true;
            }
           
            StartCoroutine(ButtonRecover(Buttons[5], RecoverTime));
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) || inputindex == 6)
        {
            Buttons[6].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                audioSource[6].gameObject.SetActive(!audioSource[6].gameObject.activeSelf);
                triggered = true;
            }
            StartCoroutine(ButtonRecover(Buttons[6], RecoverTime));
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) || inputindex == 7)
        {
            Buttons[7].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                laserEffect.SetActive(!laserEffect.activeSelf);
                triggered = true;
            }
            
            StartCoroutine(ButtonRecover(Buttons[7], RecoverTime));
        }
        if (inputindex == 8)
        {
            Buttons[8].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                selfSpin.spin();
            }
            
            StartCoroutine(ButtonRecover(Buttons[8], RecoverTime));
        }
        if(inputindex == 9)
        {
            Buttons[9].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[9], RecoverTime));
        }
        if(inputindex == 10)
        {
            Buttons[10].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[10], RecoverTime));
        }
        if(inputindex == 11)
        {
            Buttons[11].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[11], RecoverTime));
        }
        if(inputindex == 12)
        {
            Buttons[12].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[12], RecoverTime));
        }
        if(inputindex == 13)
        {
            Buttons[13].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[13], RecoverTime));
        }
        if(inputindex == 14)
        {
            Buttons[14].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[14], RecoverTime));
        }
        if(inputindex == 15)
        {
            Buttons[15].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[15], RecoverTime));
        }
        if(inputindex == 16)
        {
            Buttons[16].GetComponent<Renderer>().material.SetColor("_Color", TriggerColor);
            if (!triggered)
            {
                
            }
            
            StartCoroutine(ButtonRecover(Buttons[16], RecoverTime));
        }


        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
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
        Button.GetComponent<Renderer>().material.SetColor("_Color", OriginalColor);      

    }
    void SetUpNoteAction(int index)
    {
        var action = _noteActions[index];
        action.performed += (ctx) => OnNotePerformed(ctx, index);
        action.canceled += (ctx) => OnNoteCanceled(ctx, index);
        action.Enable();
    }
    void OnNotePerformed(InputAction.CallbackContext ctx, int index)
    {
        Debug.Log("Input" + index + "");
        inputindex = index;       
    }
    void OnNoteCanceled(InputAction.CallbackContext ctx, int index)
    {
        inputindex = -1;
        triggered = false;
    }
    public void GameStart()
    {
        startProcess += activeSpeed * Time.deltaTime;
        if(startProcess >= 100)
        {
           //LightOn.SetBool("LightOn", true);
            mainMusic.Play();
            startButton.SetActive(false);
            bPMSpawner.trigger = true;
            decreasSpeed = 0;
        }
    }
}
