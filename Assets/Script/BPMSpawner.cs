using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class GenerterObject
{
    public GameObject prefab; // 要生成的Prefab
    public Transform generateTransform;
    public float[] generatePosition;
}

public class BPMSpawner : MonoBehaviour
{
    [Header("生成設定")]
    public List<GenerterObject> generterObject = new List<GenerterObject>();
    [Header("鏡頭")]
    public Animator cameraAnima;
    
    [Header("音效")]
    public AudioSource tik;
    public AudioClip tok;
    public AudioSource mainMusic;
    public AudioSource[] music;
    [Header("BPM 設定")]
    public float bpm = 60; // 每分鐘生成數量
    public double offset;
    public double delay;
    public double startTime;
    public int counter;
    public double tikTime;
    private float interval; // 每次生成間隔秒數
    public double timer;
    public double realTime;
    public KeyCode keyCode;
    public bool trigger{get; set;}
    [Header("Event 設定")]
    public UnityEvent onTick;

    void Start()
    {
        cameraAnima.speed = cameraAnima.speed * (bpm / 120);
        // BPM 轉換成秒數間隔
        timer = 0f;
        interval = 60f / bpm;
        counter = 1;
        startTime = Time.time + delay;
        timer -= offset;
    }

    void Update()
    {
        bpm = Mathf.Clamp(bpm, 0, int.MaxValue);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            trigger = !trigger;
        }
        if (trigger)
        {
            interval = 60f / bpm;
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                tikTime = interval;
                realTime = timer - startTime;
                delay = tikTime - realTime;
                // time_1 = interval + deltaDelay;
                if (tik != null)
                    tik.PlayOneShot(tok);
                counter++;
                timer = 0;
                if (delay <= 0.001f)
                {
                    timer -= delay;
                }

                onTick?.Invoke();
                
                foreach(AudioSource _music in music)
                {
                    if(!_music.isPlaying && _music.gameObject.activeSelf)
                    {
                        _music.pitch = bpm / 145f;
                        _music.Play();
                    }
                    
                }
                foreach(var _generterObject in generterObject)
                {
                    SpawnPrefab(_generterObject);                    
                }

            }
        }
        
    }

    void SpawnPrefab(GenerterObject _generterObject)
    {
        // 從四個X值中挑一個
        float[] xs = _generterObject.generatePosition;
        float chosenX = xs[Random.Range(0, xs.Length)];
        Transform _transform = _generterObject.generateTransform;

        Vector3 spawnPos = new Vector3(
            _transform.position.x,
            _transform.position.y,
            _transform.position.z + chosenX
        );

        Instantiate(_generterObject.prefab, spawnPos, Quaternion.identity);
    }
}
