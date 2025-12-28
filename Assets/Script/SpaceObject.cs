using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    public float baseSpeed = 1f;
    public float rotationSpeed = 30f;

    float currentSpeed;
    Vector3 moveDir;

    void Start()
    {
        moveDir = Random.onUnitSphere;
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        transform.position += moveDir * currentSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void SetWarp(bool isWarp, float warpMultiplier)
    {
        currentSpeed = isWarp ? baseSpeed * warpMultiplier : baseSpeed;

        // Warp 時拉長物件
        transform.localScale = isWarp
            ? new Vector3(1, 1, 4)
            : Vector3.one;
    }
}
