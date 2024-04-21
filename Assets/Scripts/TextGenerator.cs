using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextGenerator : MonoBehaviour
{
    public static TextGenerator instance;

    public string sharedString;

    string[] textArray = new string[10];
    float[] xCoords = {1f, 3f, 5f, 7f, 9f, 11f, 13f, 15f, 17f, 19f};
    float[] yCoords = {-0.5f,-0.5f,-0.5f,-0.5f,-0.5f,-0.5f,-0.5f,-0.5f,-0.5f,-0.5f,};

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
        if (!string.IsNullOrEmpty(GlobalVariables.instance.AITextResult))
        {
            if(SceneManager.GetActiveScene().name != "Start Map")
            {
                CreateTextObject();
                GlobalVariables.instance.AITextResult = ""; // Clear the string after creating the object
            }
        }
        if (!string.IsNullOrEmpty(sharedString))
        {
            if(SceneManager.GetActiveScene().name != "Start Map")
            {
                CreateTextSide();
                sharedString = ""; // Clear the string after creating the object
            }
        }
    }

    void CreateTextObject()
    {
        // Create a Text object
        GameObject textGO = new GameObject("TextObject");
        TextMesh textMesh = textGO.AddComponent<TextMesh>();
        textMesh.text = GlobalVariables.instance.AITextResult;
        //this is where we parse the output from the model into chunks
        textArray = parseInput(textMesh.text);
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

        float groundLevel = -2.5f;

        Camera mainCamera = Camera.main;
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float minY = groundLevel + textBounds.extents.y;
        float maxY = 0f;
        Vector3 randomPosition = new Vector3(
            Random.Range(bottomLeft.x, topRight.x),
            Random.Range(minY, maxY),
            0
        );

        textGO.transform.position = randomPosition;
    }

    void CreateTextSide()
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
    }

    public void ReadStringInput(string s)
    {
        sharedString = s;
        Debug.Log(sharedString);
        GlobalVariables.instance.message = s;
    }

    //takes in string input, and slices the sentence into words which are then randomly? selected to be used in platform generation
    public string[] parseInput(string s)
    {
        string[] temp = new string[15];
        s.Split(' ');
        return temp;
    }

}
