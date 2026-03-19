using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 確保掛載此腳本的物件上一定有 Animator 元件
[RequireComponent(typeof(Animator))]
public class RandomWaveController : MonoBehaviour
{
    private Animator animator;

    [Header("Look At Settings (面向目標設定)")]
    [Tooltip("要面向的目標物件")]
    public Transform target;
    
    [Tooltip("鎖定 X 軸旋轉（打勾則保持原本的 X 軸角度，通常用於避免角色上下傾斜）")]
    public bool lockXRotation = true;
    
    [Tooltip("鎖定 Y 軸旋轉（打勾則保持原本的 Y 軸角度）")]
    public bool lockYRotation = false;
    
    [Tooltip("鎖定 Z 軸旋轉（打勾則保持原本的 Z 軸角度，通常用於避免角色左右傾倒）")]
    public bool lockZRotation = true;

    [Tooltip("旋轉偏移量 (X, Y, Z)，用來微調最終的面向角度")]
    public Vector3 rotationOffset;

    void Start()
    {
        // 1. 先處理面向目標的邏輯
        if (target != null)
        {
            LookAtTarget();
        }

        // 2. 處理原本的動畫邏輯
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

    private void LookAtTarget()
    {
        // 計算朝向目標的向量
        Vector3 direction = target.position - transform.position;
        
        // 避免目標和自己位置完全重疊導致計算出錯
        if (direction == Vector3.zero) return;

        // 取得如果完全面向目標時，應該要有的旋轉值
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        Vector3 currentEulerAngles = transform.eulerAngles;

        // 根據 Inspector 中的設定，決定要用「目標的角度」還是「原本的角度」
        float finalX = lockXRotation ? currentEulerAngles.x : targetEulerAngles.x;
        float finalY = lockYRotation ? currentEulerAngles.y : targetEulerAngles.y;
        float finalZ = lockZRotation ? currentEulerAngles.z : targetEulerAngles.z;

        // 套用最終計算出來的旋轉角度，並加上自訂的偏移量 (Offset)
        transform.rotation = Quaternion.Euler(
            finalX + rotationOffset.x, 
            finalY + rotationOffset.y, 
            finalZ + rotationOffset.z
        );
    }
}