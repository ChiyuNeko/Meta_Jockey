using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_Manerger : MonoBehaviour
{
    public AudioSource clap;
    public GameObject prefab;
    public Vector3 generateZone;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            clap.Play();
            for(int i = 0; i < 10; i++)
            {
                Vector3 prefabPos = new Vector3(Random.Range(0,generateZone.x),Random.Range(0,generateZone.y),Random.Range(0,generateZone.z));
                Instantiate(prefab, prefabPos + gameObject.transform.position, Quaternion.identity);
            }
        }
    }
    private void OnDrawGizmos()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1f, 0f, 0f, 0.7f);

        // Draw the cube.
        Gizmos.DrawCube(transform.position, new Vector3(10,10,10));


    }
}
