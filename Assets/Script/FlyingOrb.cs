using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Animator;

public class FlyingOrb : MonoBehaviour
{
    [Header("要生成的特效 Prefab")]
    public GameObject effectPrefab;
    [Header("移動設定")]
    public float moveSpeed = 2f;    // 上升速度
    public float lifeTime = 3f;     // 存活時間（秒）
    private float timer;
    [Header("戳泡泡參數")]
    public Animator Orb_explode;
    public AudioSource popSFX;
    
    private bool hit;
    public string targetTag = "Controller"; 


    void Start(){
        Orb_explode = GetComponent<Animator>();
        hit=false;

    }

    void Update()
    {
        if(hit == false){
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }
        // 向上移動

        // 倒數刪除
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Orb_explode.SetBool("IShrink_No_Vfx", true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (Orb_explode != null)
            {
                BubbleCounter.bubbleCounter.BubbleCount++;
                Debug.Log("Oh fuck");
                hit=true;
                Orb_explode.SetBool("IsShrink", true);
                Orb_explode.Play("Shrink");
                popSFX.PlayOneShot(popSFX.clip);
            }
        }
    }
    public void SpawnProjectile(){

        Instantiate(effectPrefab, transform.position, Quaternion.identity);
    }

    void OnDestroy()
    {
        // 確保連子物件或特效都被清乾淨
        foreach (Transform child in transform)
        {
            if (child != null)
                Destroy(child.gameObject);
        }
    }
    public void Died(){

        Destroy(gameObject); 
    }
}
