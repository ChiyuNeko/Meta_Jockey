using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{
    public TextMeshPro content;
    // Start is called before the first frame update
    public void SetContent(string text)
    {
        content.text = text;
    }
}
