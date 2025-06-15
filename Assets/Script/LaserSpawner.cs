using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject laserPrefab;
    public float radius = 2f;
    public float rotationSpeed = 45f;

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            float angle = i * 120f;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 pos = transform.position + rotation * Vector3.forward * radius;

            GameObject laser = Instantiate(laserPrefab, pos, rotation, transform);

            // 設定每條雷射的顏色 & 速度
            var mover = laser.GetComponent<LaserMover>();
            if (mover != null)
            {
                mover.rotationSpeed = rotationSpeed;
                mover.beamLength = 15f;

                mover.colorGradient = GenerateRandomGradient(i);
            }
        }
    }

    Gradient GenerateRandomGradient(int index)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        Color color;
        switch (index)
        {
            case 0: color = Color.red; break;
            case 1: color = Color.green; break;
            case 2: color = Color.blue; break;
            default: color = Color.white; break;
        }

        colorKeys[0].color = color;
        colorKeys[0].time = 0f;
        colorKeys[1].color = Color.white;
        colorKeys[1].time = 1f;

        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 1f;

        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }
}
