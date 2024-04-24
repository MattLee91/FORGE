using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class TextGenerator : MonoBehaviour
{
    public static TextGenerator instance;
    public bool loaded = false;

    public string sharedString;

    int platCount = 0;
    string[] textArray = new string[10];
    string[] textArrayUp = new string[12];
    float[] xCoords = {0f, 7f, 14f, 18f, 23.5f, 33f, 28f, 34f, 39f, 46.5f};
    float[] yCoords = {-0.5f,2f,5f,2.5f,-1.5f,-0.5f,2f,3.5f,4.5f,0.5f};
    float[] xCoordsUp = {-10f, -5.83f, -12f, -24.4f, -20.1f, -14.5f, -5.18f, -6.32f, -11.26f, -15.6f, -19.9f, -16.2f};
    float[] yCoordsUp = {0f, 2.5f, 5.4f, 10.4f, 13.3f, 15.3f, 17f, 19.8f, 21.25f, 23f, 24.8f, 42f};

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

    void Update()
    {
        if (!string.IsNullOrEmpty(GlobalVariables.instance.AITextResult) && !loaded)
        {
            if(SceneManager.GetActiveScene().name == "Start Map 1")
            {
                Debug.Log("Creating horizontal text");
                CreateTextSide();
                loaded = true;
                //GlobalVariables.instance.AITextResult = ""; // Clear the string after creating the object
            }
            if(SceneManager.GetActiveScene().name == "Upscroll Map")
            {
                Debug.Log("Creating vertical text");
                CreateTextUp();
                loaded = true;
            }
        }
        /*if (!string.IsNullOrEmpty(sharedString))
        {
            if(SceneManager.GetActiveScene().name != "Start Map")
            {
                CreateTextSide();
                sharedString = ""; // Clear the string after creating the object
            }
        }*/
    }

    void CreateTextSide()
    {
        platCount = 10;
        textArray = parseInput(GlobalVariables.instance.AITextResult);
        for(int i=0; i<textArray.Length;i++)
        {
            GameObject textGO = new GameObject("TextObject");
            TextMesh textMesh = textGO.AddComponent<TextMesh>();
            textMesh.text = textArray[i];
            textMesh.fontSize = 9;

        // Create a Box Collider that fits the text
            Bounds textBounds = new Bounds(textMesh.transform.position, Vector3.zero);
            MeshRenderer meshRenderer = textGO.GetComponent<MeshRenderer>();
            textBounds = meshRenderer.bounds;

            BoxCollider2D boxCollider2D = textGO.AddComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(textBounds.size.x, textBounds.size.y);
            boxCollider2D.offset = new Vector2(textBounds.center.x, textBounds.center.y);
            boxCollider2D.gameObject.layer = LayerMask.NameToLayer("Ground");
            boxCollider2D.gameObject.tag = "Word";

            Rigidbody2D rigidbody2D = textGO.AddComponent<Rigidbody2D>();
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            rigidbody2D.gameObject.layer = LayerMask.NameToLayer("Ground");
            rigidbody2D.gameObject.tag = "Word";

            MeshRenderer textRenderer = textGO.GetComponent<MeshRenderer>();
            int platformLayerID = SortingLayer.NameToID("Platforms");
            textRenderer.sortingLayerID = platformLayerID;
            textRenderer.sortingOrder = 0; // Adjust this value to control the sorting order

            Vector3 setPosition = new Vector3(xCoords[i],yCoords[i],0);

            textGO.transform.position = setPosition;
        }
    }

    void CreateTextUp()
    {
        platCount = 12;
        textArrayUp = parseInput(GlobalVariables.instance.AITextResult);
        Debug.Log("Finished parsing input");
        for(int i=0; i<textArrayUp.Length;i++)
        {
            GameObject textGO = new GameObject("TextObject");
            TextMesh textMesh = textGO.AddComponent<TextMesh>();
            textMesh.text = textArrayUp[i];
            textMesh.fontSize = 9;

        // Create a Box Collider that fits the text
            Bounds textBounds = new Bounds(textMesh.transform.position, Vector3.zero);
            MeshRenderer meshRenderer = textGO.GetComponent<MeshRenderer>();
            textBounds = meshRenderer.bounds;

            BoxCollider2D boxCollider2D = textGO.AddComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(textBounds.size.x, textBounds.size.y);
            boxCollider2D.offset = new Vector2(textBounds.center.x, textBounds.center.y);
            boxCollider2D.gameObject.layer = LayerMask.NameToLayer("Ground");
            boxCollider2D.gameObject.tag = "Word";

            Rigidbody2D rigidbody2D = textGO.AddComponent<Rigidbody2D>();
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            rigidbody2D.gameObject.layer = LayerMask.NameToLayer("Ground");
            rigidbody2D.gameObject.tag = "Word";

            MeshRenderer textRenderer = textGO.GetComponent<MeshRenderer>();
            int platformLayerID = SortingLayer.NameToID("Platforms");
            textRenderer.sortingLayerID = platformLayerID;
            textRenderer.sortingOrder = 0; // Adjust this value to control the sorting order

            Vector3 setPosition = new Vector3(xCoordsUp[i],yCoordsUp[i],0);

            textGO.transform.position = setPosition;
        }
    }

    /*void CreateTextSide()
    {

        for(int i=0; i<textArray.Length; i++)
        {
            GameObject textGO = new GameObject("TextObject");
            TextMesh textMesh = textGO.AddComponent<TextMesh>();
            textMesh.text = textArray[i];
            textMesh.fontSize = 10;

        // Create a Box Collider that fits the text
            Bounds textBounds = new Bounds(textMesh.transform.position, Vector3.zero);
            MeshRenderer meshRenderer = textGO.GetComponent<MeshRenderer>();
            textBounds = meshRenderer.bounds;

            BoxCollider2D boxCollider2D = textGO.AddComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(textBounds.size.x, textBounds.size.y);
            boxCollider2D.offset = new Vector2(textBounds.center.x, textBounds.center.y);
            boxCollider2D.gameObject.layer = LayerMask.NameToLayer("Ground");

            MeshRenderer textRenderer = textGO.GetComponent<MeshRenderer>();
            int platformLayerID = SortingLayer.NameToID("Platforms");
            textRenderer.sortingLayerID = platformLayerID;
            textRenderer.sortingOrder = 0; // Adjust this value to control the sorting order

            Vector3 setPosition = new Vector3(xCoords[i],yCoords[i],0);

            textGO.transform.position = setPosition;
        }
    }*/

    public void ReadStringInput(string s)
    {
        sharedString = s;
        Debug.Log(sharedString);
        GlobalVariables.instance.message = s;
    }

    //takes in string input, and slices the sentence into words which are then randomly? selected to be used in platform generation
    public string[] parseInput(string s)
    {
        string[] temp = s.Split(new char[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
        //string[] wordPairs = GroupWordsIntoPairs(temp);
        System.Random random = new System.Random();
        // Randomly select 10 words
        List<string> selectedWordPairs = new List<string>();
        int pairsCount = 0;
        while(pairsCount < platCount && temp.Length >=2)
        {
            int index = random.Next(temp.Length - 1);
            string pair = $"{temp[index]} {temp[index + 1]}";
            if (pair.Length <= 15)
            {
                selectedWordPairs.Add(pair);
                pairsCount++;
            }
            temp = temp.Where((value, idx) => idx != index && idx != index + 1).ToArray();
        }
        while (pairsCount < platCount)
        {
            selectedWordPairs.Add("");
            pairsCount++;
        }
        //string[] selectedWords = wordPairs.OrderBy(w => random.Next()).Take(10).ToArray();
        return selectedWordPairs.ToArray();
    }

    private string[] GroupWordsIntoPairs(string[] words)
    {
        // Check if there are enough words to form pairs
        if (words.Length < 2)
            throw new ArgumentException("The input sentence must contain at least 2 words to form pairs.");

        // Group the words into pairs
        string[] pairs = new string[words.Length - 1];
        for (int i = 0; i < words.Length - 1; i++)
        {
            pairs[i] = $"{words[i]} {words[i + 1]}";
        }
        return pairs;
    }
}
