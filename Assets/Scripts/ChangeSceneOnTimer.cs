using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTimer : MonoBehaviour
{
    public float changeTime;
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        changeTime -= Time.deltaTime;
        if (changeTime <= 0)
        {
            Debug.Log("Loading level with name: " + GlobalVariables.instance.levelNames[GlobalVariables.instance.levelNameCounter]);
            SceneManager.LoadScene(GlobalVariables.instance.levelNames[GlobalVariables.instance.levelNameCounter]);
        }
    }
}
