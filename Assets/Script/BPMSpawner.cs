using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMSpawner : MonoBehaviour
{
    [Header("生成設定")]
    public GameObject prefab; // 要生成的Prefab
    public float x1;
    public float x2;
    public float x3;
    public float x4;
    [Header("音效")]
    public AudioSource tik;
    public AudioClip tok;
    public AudioSource mainMusic;
    public AudioSource[] music;
    [Header("BPM 設定")]
    public int bpm = 60; // 每分鐘生成數量
    public double delay;
    public double startTime;
    public int counter;
    public double tikTime;
    private float interval; // 每次生成間隔秒數
    public double timer;
    public double realTime;
    public KeyCode keyCode;
    bool trigger;

    void Start()
    {
        // BPM 轉換成秒數間隔
        timer = 0f;
        interval = 60f / bpm;
        counter = 1;
        startTime = Time.time + delay;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            trigger = !trigger;
            mainMusic.Play();
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

                foreach(AudioSource _music in music)
                {
                    if(!_music.isPlaying && _music.gameObject.activeSelf)
                    {
                        _music.pitch = bpm / 145f;
                        _music.Play();
                    }
                    
                }

                SpawnPrefab();
            }
        }
        
    }

    void SpawnPrefab()
    {
        // 從四個X值中挑一個
        float[] xs = { x1, x2, x3, x4 };
        float chosenX = xs[Random.Range(0, xs.Length)];

        Vector3 spawnPos = new Vector3(
            gameObject.transform.localPosition.x,
            gameObject.transform.localPosition.y,
            gameObject.transform.localPosition.z + chosenX
        );

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
