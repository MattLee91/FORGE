using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 15f;
    Rigidbody2D myRigidbody;

    bool isStuck = false;
    PlayerMovement player;
    float xSpeed;

    GameObject bullClone;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
        transform.localScale = new Vector2((Mathf.Sign(xSpeed)), 1f );

        
    }

    void Update()
    {   if(!isStuck){
            myRigidbody.velocity = new Vector2 (xSpeed, 0f);
        }

    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.collider.tag != "Player"){
        myRigidbody.bodyType = RigidbodyType2D.Static;
        isStuck = true;
       }
    }
}
