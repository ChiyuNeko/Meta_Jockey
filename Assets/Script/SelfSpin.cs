using UnityEngine;

public class SelfSpin : MonoBehaviour
{
    public KeyCode spinKey;
    public float spinSpeed = 50f;
    public float spinAngle = 90f;
    public Vector3 angle;
    public int counter;
    float currentAngle;
    float nextAngle;
    [Header("材質設定")]
    public Material material;
    public float initialAlpha = 0.178f;
    public float alphaScale = 0.178f;
    public float decreasSpeed = 1.0f;
    void Start()
    {
        angle = transform.rotation.eulerAngles;
        nextAngle = 0f;
    }
    void Update()
    {
        if(Input.GetKeyDown(spinKey))
        {
            spin();
        }

        if( currentAngle < nextAngle)
        {            
            currentAngle += Time.deltaTime * spinSpeed;
            transform.localRotation = Quaternion.Euler(Vector3.forward * currentAngle);
        }

        if (material != null)
            material.SetFloat("_Alpha", alphaScale);
        alphaScale = Mathf.Lerp (alphaScale, 0, Time.deltaTime * decreasSpeed);

    }
    public void spin()
    {
        counter++;
        nextAngle += spinAngle;
        alphaScale = initialAlpha;
    }
}
