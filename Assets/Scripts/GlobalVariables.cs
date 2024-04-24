using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables instance;

    public bool isInTextInput = false;
    public bool isInAnvilArea = false;
    public bool hasIntWithAnvil1 = false;
    public string AITextResult = "";
    public string message = "";

    public int levelCount = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}