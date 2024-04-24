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
    public int levelCount = 1;
    public string[] levelNames = {"Start Map 1", "Upscroll Map", "BossBattle", "Falling Map", "End Map"};
    public int levelNameCounter = 0;

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