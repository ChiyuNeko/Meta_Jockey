using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSwitch : MonoBehaviour
{
    public GameObject gameObject;

    public void SwitchActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
