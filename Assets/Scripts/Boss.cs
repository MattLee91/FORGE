using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject enemy;

    PlayerMovement player;
    public GameObject bossStarter;
    [SerializeField] GameObject boss;
    [SerializeField] Transform spawn;
    [SerializeField] Transform spawn1;
    [SerializeField] Transform spawn2;
    [SerializeField] Transform spawn3;

    [SerializeField] Transform spawn4;

    [SerializeField] Transform spawnBoss;
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        // Instantiate(enemy, spawn.position, transform.rotation);
        // Instantiate(enemy, spawn1.position, transform.rotation);
        // Instantiate(enemy, spawn2.position, transform.rotation);
        // Instantiate(enemy, spawn3.position, transform.rotation);
    }

   
    // Update is called once per frame
    void Update()
    {
        
    }

    void  OnTriggerEnter2D(Collider2D other) {
        Instantiate(enemy, spawn.position, transform.rotation);
        Instantiate(enemy, spawn1.position, transform.rotation);
        Instantiate(enemy, spawn2.position, transform.rotation);
        Instantiate(enemy, spawn3.position, transform.rotation);
        Instantiate(enemy, spawn4.position, transform.rotation);
        Instantiate(boss, spawnBoss.position, transform.rotation);
        StartCoroutine(waiter());

    }


IEnumerator waiter()
{
    bossStarter.SetActive(true);
    player.isAlive = false;
    player.moveInput = new Vector2(0f, 0f);
    //So player doesn't move during view of scene
    yield return new WaitForSeconds(3);
    player.isAlive = true;
    bossStarter.SetActive(false);
    Destroy(gameObject);
}



}
