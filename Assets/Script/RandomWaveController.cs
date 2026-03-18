using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 確保掛載此腳本的物件上一定有 Animator 元件
[RequireComponent(typeof(Animator))]
public class RandomWaveController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // 取得物件上的 Animator 元件
        animator = GetComponent<Animator>();

        // Random.Range(0, 2) 在整數模式下，會隨機回傳 0 或 1 (不包含最大值 2)
        int randomAction = Random.Range(0, 2);

        if (randomAction == 0)
        {
            // 抽到 0：觸發往前揮手
            animator.SetBool("IsWaveForward", true);
            animator.SetBool("IsLeftAndRight", false); 
        }
        else
        {
            // 抽到 1：觸發左右揮手
            animator.SetBool("IsLeftAndRight", true);
            animator.SetBool("IsWaveForward", false); 
        }
    }
}