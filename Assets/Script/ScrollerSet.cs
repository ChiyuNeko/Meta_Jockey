using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollerSet : MonoBehaviour
{
    public ScrollRect scrollRect;
    void Update()
    {
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom() 
    { 
        yield return new WaitForEndOfFrame(); 
        scrollRect.gameObject.SetActive(true);
        scrollRect.verticalNormalizedPosition = 0f;       
    }
}
