using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class AcceleratingProjectile : MonoBehaviour
{
    [Header("特效設定 (VFX Settings)")]
    [Tooltip("主要 VFX 物件 (例如掛在子物件的引擎推進器)")]
    public GameObject mainVfxObject;
    
    [Tooltip("第二階段 VFX 預製物 (Prefab) (時間到時會在此物件當下位置生成)")]
    public GameObject secondVfxPrefab;

    [Header("時間軸設定 (Timeline Settings)")]
    [Tooltip("1. 起飛前的等待時間 (秒)")]
    public float delayBeforeLaunch = 2f;
    
    [Tooltip("2. 引擎持續運作的時間 (秒) - 決定何時熄火")]
    public float engineBurnTime = 5f;

    [Tooltip("3. 熄火後經過多久生成第二個 VFX (秒)")]
    public float delayForSecondVFX = 1f;

    [Tooltip("4. 熄火後經過多久自毀本體 (秒) - 必須大於等於生成第二個VFX的時間")]
    public float timeToDestroyAfterBurnout = 3f;

    [Header("移動設定 (Movement Settings)")]
    [Tooltip("飛行方向 (X, Y, Z)")]
    public Vector3 flightDirection = new Vector3(0, 1, 0); 
    
    [Tooltip("初始速度")]
    public float initialSpeed = 0f;
    [Tooltip("最大速度")]
    public float maxSpeed = 20f;
    [Tooltip("加速度 (每秒增加的速度)")]
    public float acceleration = 5f;
    [Tooltip("熄火後的減速度 (每秒減少的速度)")]
    public float deceleration = 10f;

    // 內部變數
    private float currentSpeed;
    private Vector3 normalizedDirection;
    
    // 定義目前的狀態
    private enum State { Waiting, Accelerating, Decelerating }
    private State currentState = State.Waiting;

    void Start()
    {
        normalizedDirection = flightDirection.normalized;
        currentSpeed = initialSpeed;

        // 啟動主流程協程
        StartCoroutine(FlightSequenceRoutine());
    }

    void Update()
    {
        if (currentState == State.Waiting) return; 

        // --- 處理速度變化邏輯 ---
        if (currentState == State.Accelerating)
        {
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed); 
            }
        }
        else if (currentState == State.Decelerating)
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f); 
            }
        }

        // --- 處理移動邏輯 ---
        transform.Translate(normalizedDirection * currentSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 控制整個飛行生命週期的協程
    /// </summary>
    private IEnumerator FlightSequenceRoutine()
    {
        // 【階段一：等待起飛】
        if (delayBeforeLaunch > 0)
        {
            yield return new WaitForSeconds(delayBeforeLaunch);
        }

        // 【階段二：點火起飛並加速】
        currentState = State.Accelerating;
        OnLaunchAction(); 
        
        yield return new WaitForSeconds(engineBurnTime);

        // 【階段三：引擎熄火並減速】
        currentState = State.Decelerating;
        OnBurnoutAction();

        // --- 計算生成特效與自毀的時間差 ---
        float actualVfxDelay = Mathf.Clamp(delayForSecondVFX, 0f, timeToDestroyAfterBurnout);
        float remainingTimeBeforeDestruct = timeToDestroyAfterBurnout - actualVfxDelay;

        // 【等待生成第二階段特效】
        if (actualVfxDelay > 0)
        {
            yield return new WaitForSeconds(actualVfxDelay);
        }
        
        OnPlaySecondVFXAction();

        // 【等待剩下的時間直至自毀】
        if (remainingTimeBeforeDestruct > 0)
        {
            yield return new WaitForSeconds(remainingTimeBeforeDestruct);
        }
        
        // 【階段四：自毀】
        OnDestructAction();
    }

    /// <summary>
    /// 起飛瞬間觸發
    /// </summary>
    private void OnLaunchAction()
    {
        if (mainVfxObject != null)
        {
            mainVfxObject.SetActive(true);
        }
    }

    /// <summary>
    /// 引擎熄火時觸發
    /// </summary>
    private void OnBurnoutAction()
    {
        if (mainVfxObject != null)
        {
            VisualEffect vfx = mainVfxObject.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Stop(); 
            }
            else
            {
                mainVfxObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 生成第二階段 VFX 預製物時觸發
    /// </summary>
    private void OnPlaySecondVFXAction()
    {
        if (secondVfxPrefab != null)
        {
            // 在物體當下的位置 (transform.position) 與當下的旋轉角度 (transform.rotation) 生成預製物
            Instantiate(secondVfxPrefab, transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// 自毀時觸發
    /// </summary>
    private void OnDestructAction()
    {
        // 直接銷毀本體。剛才 Instantiate 出來的第二階段特效不會受到影響。
        Destroy(gameObject);
    }
}