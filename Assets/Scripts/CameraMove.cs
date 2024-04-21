using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Camera cameraToAdjust; // Reference to the camera you want to adjust
    public float targetFOV = 175f; // Target FOV value
    public float zoomDuration = 2f; // Duration of the zoom effect in seconds

    public float speed = 1f;

     Vector3 upLeft = new Vector3(-1, 1, 0);
    void Start()
    {


        // Start the zoom coroutine
        if(zoomDuration != 0 || speed != 0 ){
        StartCoroutine(ChangeFOVSmoothly());
        }
    }

     void Update() {
        //transform.Translate(upLeft * speed * Time.deltaTime);
    }
    IEnumerator ChangeFOVSmoothly()
    {
        float initialFOV = cameraToAdjust.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < zoomDuration)
        {
            // Calculate the progress of the zoom effect
            float t = elapsedTime / zoomDuration;

            // Smoothly interpolate between the initial FOV and the target FOV
            float newFOV = Mathf.Lerp(initialFOV, targetFOV, t);

            // Apply the new FOV to the camera
            cameraToAdjust.fieldOfView = newFOV;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            transform.Translate(upLeft * speed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final FOV is set to the target value
        cameraToAdjust.fieldOfView = targetFOV;
    }
}

