using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActual : MonoBehaviour
{
    // Start is called before the first frame update

    Animator myAnimator;
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void  OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon" ){
            myAnimator.SetTrigger("isHit");
        }
    }



}
