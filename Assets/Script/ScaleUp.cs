using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUp : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public float maxiam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.localScale.x <= maxiam)
            gameObject.transform.localScale += Vector3.one * Time.deltaTime * speed;
        else
            Destroy(gameObject);
    }
}
