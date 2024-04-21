using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Interacted : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    public void OnButtonClick()
    {
        Debug.Log("we looking good");
        GlobalVariables.instance.hasIntWithAnvil1 = true;
    }
}
