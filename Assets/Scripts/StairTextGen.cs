using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairTextGen : MonoBehaviour
{
    string[] stairArray = {"Jump", "Here" , "To", "Start"};
    float[] xCoords = {16, 19.5f, 22.5f, 25};
    float[] yCoords = {-0.75f, 0.25f, 1.25f, 2.25f};
    // Update is called once per frame
    void Update()
    {
        if(GlobalVariables.instance.hasIntWithAnvil1)
        {
            CreateTextObjects();
            GlobalVariables.instance.hasIntWithAnvil1 = false;
        }
    }

    void CreateTextObjects()
    {
        for(int i=0; i<stairArray.Length; i++)
        {
            GameObject textGO = new GameObject("TextObject");
        TextMesh textMesh = textGO.AddComponent<TextMesh>();
        textMesh.text = stairArray[i];
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

}
