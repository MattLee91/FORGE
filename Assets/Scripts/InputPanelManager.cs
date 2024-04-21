using System;
using System.Collections;
using System.Collections.Generic;
using LLMUnitySamples;
using UnityEngine;

public class InputPanelManager : MonoBehaviour
{
    public GameObject textInputPanel;
    private string input;
    // Start is called before the first frame update
    void Start()
    {
        textInputPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(textInputPanel.activeSelf)
        {
            GlobalVariables.instance.isInTextInput = true;
        }
        else
        {
            GlobalVariables.instance.isInTextInput = false;
        }

        /*if(GlobalVariables.instance.isInAnvilArea && Input.GetKeyDown(KeyCode.T))
        {
            if(!textInputPanel.activeSelf)
            {
                textInputPanel.SetActive(true);
            }
        }*/
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(!textInputPanel.activeSelf)
            {
                textInputPanel.SetActive(true);
            }
        }

        //if(Input.GetKeyDown(KeyCode.Return))
        //{
        //    textInputPanel.SetActive(false);
        //}
    }

    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log(input);
    }
}
