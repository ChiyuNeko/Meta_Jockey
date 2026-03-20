using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;
using UnityEngine.Audio;
using Unity.Mathematics;
using UnityEngine.VFX;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.Events;

[System.Serializable]
public class Keys
{
    public bool isSwitch;
    public bool triggerState;
    public UnityEvent onTrigger;
    public GameObject display;
    public Color OriginalColor;
    public Color TriggerColor;
}

public class HotKey : MonoBehaviour
{
    [SerializeField] InputAction[] _noteActions = null;
    [SerializeField] InputAction _modWheelAction = null;
    static string[] NoteNames = new[] { "C", "C#", "D", "D#", "E", "F",
                                        "F#", "G", "G#", "A", "A#", "B" };
    public List<Keys> keys = new List<Keys>();
    public AudioSource[] LoopSet1;
    public AudioSource[] LoopSet2;
    public AudioSource mainMusic;
    public AudioMixer audioMixer;
    public float EQ;
    public float Vol;
    public VisualEffect visualEffect;
    [Header("Controller Parameters")]
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

        foreach(Keys k in keys)
        {
            k.display.GetComponent<Renderer>().material.SetColor("_Color", k.OriginalColor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var mod = _modWheelAction.ReadValue<float>();
        Debug.Log("" + mod);

        startProcess -= decreasSpeed * Time.deltaTime;
        startProcess = Mathf.Clamp(startProcess, 0, 100);

        

        EQ = Mathf.Lerp(EQ, (mod-0.2f) * 12500, 0.1f) ;
        EQ = Mathf.Clamp(EQ, 100, 10000);
        audioMixer.SetFloat("BaseLowPass", EQ);
        
        //Vol = Mathf.Lerp(Vol, arduinoData.encoder2 * 4, 0.1f) ;
        Vol = Mathf.Clamp(Vol, -80, 10);
        audioMixer.SetFloat("Vol", Vol);      

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }

    public void LoopControl(GameObject loopAudio)
    {
        if (!triggered)
        {
            loopAudio.SetActive(!loopAudio.activeSelf);
            triggered = true;
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
        if(!keys[index].isSwitch)
        {
            if(keys[index].triggerState == false) 
            {
                keys[index].onTrigger.Invoke();
                keys[index].triggerState = true;
                keys[index].display.GetComponent<Renderer>().material.SetColor("_Color", keys[index].TriggerColor);
            }
        }
        else
        {
            keys[index].triggerState = !keys[index].triggerState;
            keys[index].onTrigger.Invoke();
            keys[index].display.GetComponent<Renderer>().material.SetColor("_Color", keys[index].TriggerColor);
        }
    }
    void OnNoteCanceled(InputAction.CallbackContext ctx, int index)
    {
        triggered = false;
        if(!keys[index].isSwitch)
        {
            keys[index].triggerState = false;
            keys[index].display.GetComponent<Renderer>().material.SetColor("_Color", keys[index].OriginalColor);
        }
        else
        {
            if(keys[index].triggerState)
            {
                keys[index].display.GetComponent<Renderer>().material.SetColor("_Color", keys[index].TriggerColor);
            }
            else
            {
                keys[index].display.GetComponent<Renderer>().material.SetColor("_Color", keys[index].OriginalColor);
            }
        }

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