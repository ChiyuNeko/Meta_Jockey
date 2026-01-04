using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_Manerger : MonoBehaviour
{
    public HandsParameters handsParameters;
    public float velocityThreshold = 1.0f;
    public float velocityLowThreshold = 1.0f;
    public AudioSource clap;
    public GameObject prefab;
    public GameObject wirePrefab;
    public float delayTime = 1f;
    public Vector3 generateZone;
    public bool hasGenerated = false;


    void Update()
    {
        if(!hasGenerated && -handsParameters.RightControllerAcceleration.z > velocityThreshold)
        {
            hasGenerated = true;
            clap.Play();
            for(int i = 0; i < 10; i++)
            {
                Vector3 prefabPos = new Vector3(Random.Range(0,generateZone.x),Random.Range(0,generateZone.y),Random.Range(0,generateZone.z));
                if(prefab)
                    Instantiate(prefab, prefabPos + gameObject.transform.position, Quaternion.identity);
            }
        }
        if(hasGenerated && -handsParameters.RightControllerAcceleration.z < velocityLowThreshold)
        {
            hasGenerated = false;
            StartCoroutine(delay(delayTime));
        }
    }

    IEnumerator delay(float time)
    {
        yield return new WaitForSeconds(time);
        if(wirePrefab)
            Instantiate(wirePrefab);
    }       
    private void OnDrawGizmosSelected()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Draw the cube.
        Gizmos.DrawCube(transform.position + generateZone / 2, generateZone);


    }
}
