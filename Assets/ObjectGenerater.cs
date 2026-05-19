using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerater : MonoBehaviour
{
    public Vector3 area;
    public List<GameObject> Obj = new List<GameObject>();
    public float objScale;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void GenerateInRandomArea()
    {
        float x = Random.Range(0,area.x);
        float y = Random.Range(0,area.y);
        float z = Random.Range(0,area.z); 
        Vector3 pos = new Vector3(x, y, z) + gameObject.transform.position;
        foreach(var i in Obj)
        {
            GameObject newObj = Instantiate(i, pos, Quaternion.identity);
            newObj.transform.localScale = Vector3.one * objScale;
            newObj.SetActive(true);
            Destroy(newObj, 5);
        }

        
    }

    IEnumerator _DelayGenerate(float time)
    {
        yield return new WaitForSeconds(time);
        GenerateInRandomArea();
    }

    public void DelayGenerate(float time)
    {
        StartCoroutine(_DelayGenerate(time));
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Draw the cube.
        Gizmos.DrawCube(transform.position + area / 2, area);
    }
}
