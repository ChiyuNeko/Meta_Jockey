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
using TMPro;

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
[System.Serializable]
public class ModWheel
{
    public string name;
    public float value;
    public GameObject sliderVisual; 
    public UnityEvent<float> setValue;
    public float lastSentValue;
}

public class HotKey : MonoBehaviour
{
    [Header("MIDI Pad")]
    [SerializeField] InputAction[] _noteActions = null;
    static string[] NoteNames = new[] { "C", "C#", "D", "D#", "E", "F",
                                        "F#", "G", "G#", "A", "A#", "B" };
    public List<Keys> keys = new List<Keys>();
    [Header("MIDI Controller")]
    public bool handleInput = false;
    [SerializeField] InputAction[] _modWheelAction = null;
    public List<ModWheel> modWheels = new List<ModWheel>();
    [Header("Audio Sources")]

    public AudioSource[] LoopSet1;
    public AudioSource[] LoopSet2;
    public AudioSource mainMusic;
    public AudioMixer audioMixer;
    public float lpEQ;
    public float hpEQ;
    public float Vol;
    [Header("VFX")]
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
    public TextMeshPro processText;
    public SelfSpin selfSpin;

    [Header("Camera Control")]
    public KeyCode cameraSwitch;
    public int currentDisplay;
    public Camera movingCamera;
    public Camera movingCamera2;
    public Camera staticCamera;

    [Header("UI Control")]
    public KeyCode chatUIswitch;
    public GameObject chatUI;
    public TextMeshProUGUI logText;

    void Start()
    {
        for (var i = 0; i < _noteActions.Length; i++) SetUpNoteAction(i);
        for (var i = 0; i < _modWheelAction.Length; i++) _modWheelAction[i].Enable();

        foreach(Keys k in keys)
        {
            k.display.GetComponent<Renderer>().material.SetColor("_Color", k.OriginalColor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < _modWheelAction.Length; i++)
        {
            if(!handleInput)
                modWheels[i].value = _modWheelAction[i].ReadValue<float>();
            if(modWheels[i].sliderVisual != null)
            {
                modWheels[i].sliderVisual.transform.GetChild(0).localPosition = new Vector3(0, modWheels[i].value * 3.3f, 0);
                modWheels[i].sliderVisual.transform.GetChild(1).GetComponent<Renderer>().material.SetFloat("_SliderOffest", modWheels[i].value);
            }
            if (modWheels[i].value != modWheels[i].lastSentValue)
            {
                Debug.Log( "mod" + i + ": " + modWheels[i].value);
                modWheels[i].setValue?.Invoke(modWheels[i].value);
                modWheels[i].lastSentValue = modWheels[i].value;
                logText.text += $"{modWheels[i].name}: {modWheels[i].value}\n";

            }
        }

        startProcess -= decreasSpeed * Time.deltaTime;
        startProcess = Mathf.Clamp(startProcess, 0, 100);
        processText.text = $"{(int)startProcess}%";

        

        lpEQ = modWheels[2].value * 10000;
        lpEQ = Mathf.Clamp(lpEQ, 100, 10000);
        audioMixer.SetFloat("BaseLowPass", lpEQ);

        hpEQ = modWheels[3].value * 5000;
        hpEQ = Mathf.Clamp(hpEQ, 10, 5000);
        //hpEQ = 10000 - hpEQ;
        audioMixer.SetFloat("BaseHighPass", hpEQ);
        
        Vol = Mathf.Clamp(Vol, -80, 10);
        audioMixer.SetFloat("Vol", Vol);      

        if(Input.GetKeyDown(KeyCode.Space))
        {
            startProcess = 100;
            GameStart();
        }

        if(Input.GetKeyDown(cameraSwitch))
        {
            currentDisplay++;
            currentDisplay %= 3;
            if(currentDisplay == 0)
            {
                movingCamera.targetDisplay = 1;
                movingCamera2.targetDisplay = 2;
                staticCamera.targetDisplay = 2;
            }
            else if(currentDisplay == 1)
            {
                movingCamera.targetDisplay = 2;
                movingCamera2.targetDisplay = 1;
                staticCamera.targetDisplay = 2;
            }
            else
            {
                movingCamera.targetDisplay = 2;
                movingCamera2.targetDisplay = 2;
                staticCamera.targetDisplay = 1;
            }
        }

        if(Input.GetKeyDown(chatUIswitch))
        {
            chatUI.SetActive(!chatUI.activeSelf);
        }
    }

    public void LoopControl(GameObject loopAudio)
    {
        if (!triggered)
        {
            loopAudio.SetActive(!loopAudio.activeSelf);
            triggered = true;
            //loopAudio.GetComponent<AudioSource>().Play();
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
                logText.text += $"Button {index} ON\n";
            }
            logText.text += $"Button {index} OFF\n";
        }
        else
        {
            keys[index].triggerState = !keys[index].triggerState;
            keys[index].onTrigger.Invoke();
            keys[index].display.GetComponent<Renderer>().material.SetColor("_Color", keys[index].TriggerColor);
            logText.text += $"Button {index} pressed\n";
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
            mainMusic.Play();
            startButton.SetActive(false);
            bPMSpawner.trigger = true;
            decreasSpeed = 0;
           //LightOn.SetBool("LightOn", true);
        }
    }
}