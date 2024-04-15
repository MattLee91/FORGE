using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextGenerator : MonoBehaviour
{
    public static TextGenerator instance;

    public string sharedString;

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
        if (!string.IsNullOrEmpty(sharedString))
        {
            CreateTextObject();
            sharedString = ""; // Clear the string after creating the object
        }
    }

    void CreateTextObject()
    {
        // Create a Text object
        GameObject textGO = new GameObject("TextObject");
        TextMesh textMesh = textGO.AddComponent<TextMesh>();
        textMesh.text = sharedString;
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

    public void ReadStringInput(string s)
    {
        sharedString = s;
        Debug.Log(sharedString);
    }

}
