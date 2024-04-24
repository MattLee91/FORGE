using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActual : MonoBehaviour
{
    // Start is called before the first frame update

    Animator myAnimator;

 
    PlayerMovement player;
    public GameObject boss;
    Camera bossPlease;


    void Start()
    {
        myAnimator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerMovement>();
        if(player.bossIteration == 0){
        boss = player.boss;
        }
        else if (player.bossIteration == 1){
            boss = player.boss2;
        }
        else {
            boss = player.boss3;
            player.bossIteration++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void  OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon" && player.bossIteration < 3){

            player.bossIteration++;
            StartCoroutine(waiter2());
            myAnimator.SetTrigger("isHit");
            
        }
        else if (other.tag == "Weapon"){
            StartCoroutine(waiter2());
            myAnimator.SetTrigger("isHit");
            myAnimator.SetTrigger("isDead");
        }
        
    }

    IEnumerator waiter2()
{
    boss.SetActive(true);
    
    player.isAlive = false;
    player.moveInput = new Vector2(0f, 0f);
    //So player doesn't move during view of scene
    yield return new WaitForSeconds(3);
    player.isAlive = true;
    boss.SetActive(false);
    player.createBounce();
    Destroy(gameObject);


}


}
