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
    public AudioMixer audioMixer;
    public float EQ;
    public float Vol;
    public ArduinoData arduinoData;
    public int inputindex;
    public ParticleSystem[] particleSystems;
    public VisualEffect visualEffect;
    [Header("Controller Parameters")]
    public List<GameObject> Buttons = new List<GameObject>();
    public Material OriginalColor;
    public Material TriggerColor;
    public float RecoverTime;
    public int LoopCount;
    public GameObject[] BigCircle;
    public float BigCircle1Rot;
    public float BigCircle2Rot;
    public bool triggered = false;
    public Animator LightOn;
    void Start()
    {
        for (var i = 0; i < NoteNames.Length; i++) SetUpNoteAction(i);
        _modWheelAction.Enable();
        inputindex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        var mod = _modWheelAction.ReadValue<float>();
        Debug.Log("" + mod);
        // switch (LoopCount)
        // {
        //     case 0:
        //         if (BigCircle[0].activeSelf)
        //         {
        //             foreach (GameObject i in BigCircle)
        //             {
        //                 i.SetActive(false);
        //             }
        //         }
        //         break;
        //     case 1:
        //         if(!BigCircle[0].activeSelf)
        //             BigCircle[0].SetActive(true);
        //         if(BigCircle[1].activeSelf)
        //             BigCircle[1].SetActive(false);
        //         break;

        //     case 2:
        //         if(!BigCircle[1].activeSelf)
        //             BigCircle[1].SetActive(true);
        //         if(BigCircle[2].activeSelf)
        //             BigCircle[2].SetActive(false);
        //         break;

        //     case 3:
        //         if(!BigCircle[2].activeSelf)
        //             BigCircle[2].SetActive(true);
        //         break;

        //     default:
        //         break;
        // }
        BigCircle1Rot = Mathf.Lerp(BigCircle1Rot, arduinoData.encoder, 0.01f);
        BigCircle[0].transform.rotation = quaternion.Euler(0, 90, BigCircle1Rot);
        BigCircle[1].transform.Rotate(0, 0, (arduinoData.encoder2 + 20) * Time.deltaTime * 3);

        EQ = Mathf.Lerp(EQ, mod * 10000, 0.1f) ;
        EQ = Mathf.Clamp(EQ, 100, 10000);
        audioMixer.SetFloat("BaseLowPass", EQ);
        
        Vol = Mathf.Lerp(Vol, arduinoData.encoder2 * 4, 0.1f) ;
        Vol = Mathf.Clamp(Vol, -80, 10);
        audioMixer.SetFloat("Vol", Vol);
        
        if (Input.GetKeyDown(KeyCode.Keypad1) || inputindex == 0)
        {
            Buttons[0].GetComponent<Renderer>().material = TriggerColor;
            audioSource[0].Play();
            StartCoroutine(ButtonRecover(Buttons[0], RecoverTime));
            visualEffect.Play();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2) || inputindex == 1)
        {
            Buttons[1].GetComponent<Renderer>().material = TriggerColor;
            audioSource[1].Play();
            StartCoroutine(ButtonRecover(Buttons[1], RecoverTime));
            particleSystems[0].Play();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3) || inputindex == 2)
        {
            Buttons[2].GetComponent<Renderer>().material = TriggerColor;
            audioSource[2].Play();
            StartCoroutine(ButtonRecover(Buttons[2], RecoverTime));
            visualEffect.Play();
        }

        if (Input.GetKeyDown(KeyCode.Keypad4) || inputindex == 3)
        {
            Buttons[3].GetComponent<Renderer>().material = TriggerColor;
            audioSource[3].Play();
            StartCoroutine(ButtonRecover(Buttons[3], RecoverTime));
            particleSystems[0].Play();
        }

        if (Input.GetKeyDown(KeyCode.Keypad5) || inputindex == 4)
        {
            Buttons[4].GetComponent<Renderer>().material = TriggerColor;
            if (!triggered)
            {
                audioSource[4].gameObject.SetActive(!audioSource[4].gameObject.activeSelf);
                triggered = true;
            }
            StartCoroutine(ButtonRecover(Buttons[4], RecoverTime));
        }

        if (Input.GetKeyDown(KeyCode.Keypad6) || inputindex == 5)
        {
            Buttons[5].GetComponent<Renderer>().material = TriggerColor;
            if (!triggered)
            {
                audioSource[5].gameObject.SetActive(!audioSource[5].gameObject.activeSelf);
                triggered = true;
            }
           
            StartCoroutine(ButtonRecover(Buttons[5], RecoverTime));
        }

        if (Input.GetKeyDown(KeyCode.Keypad7) || inputindex == 6)
        {
            Buttons[6].GetComponent<Renderer>().material = TriggerColor;
            if (!triggered)
            {
                audioSource[6].gameObject.SetActive(!audioSource[6].gameObject.activeSelf);
                triggered = true;
            }
            StartCoroutine(ButtonRecover(Buttons[6], RecoverTime));
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

        string nowMaterial = Button.GetComponent<Renderer>().material.name;
        Debug.Log("" + nowMaterial + TriggerColor.name);

        if (nowMaterial == TriggerColor.name + " (Instance)")
        {
            Button.GetComponent<Renderer>().material = OriginalColor;
        }

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
        LightOn.SetBool("LightOn", true);
    }
}
