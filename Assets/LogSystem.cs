using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogSystem : MonoBehaviour
{
    public TextMeshProUGUI logText;

    public void AddLog(string log)
    {
        logText.text += $"{log}\n";
    } 
}
