using UnityEngine;

/// <summary>
/// 讓遊戲物件依指定軸轉動，並且可以由當前轉速漸變到指定轉速。
/// </summary>
public class RotateObject : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // 轉軸
    [SerializeField] private float currentSpeed = 100f; // 初始轉速
    [SerializeField] private float targetSpeed = 500f; // 目標轉速
    [SerializeField] private float smoothFactor = 2f; // 轉速變化平滑度，越大變化越快
    
    private void Update()
    {
        // 平滑地由當前轉速變化到指定轉速
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothFactor);
        
        // 依指定軸轉動
        transform.Rotate(rotationAxis.normalized * currentSpeed * Time.deltaTime);
    }
}
