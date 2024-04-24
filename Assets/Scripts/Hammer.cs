using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy"){
            Destroy(other.gameObject);
        }
        if(other.tag == "Word")
        {
            Destroy(other.gameObject);
        }
        //Destroy(gameObject);
    }
}
