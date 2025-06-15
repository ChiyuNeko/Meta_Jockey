using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserMover : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float beamLength = 10f;
    public Gradient colorGradient;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.colorGradient = colorGradient;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.forward * beamLength;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}

