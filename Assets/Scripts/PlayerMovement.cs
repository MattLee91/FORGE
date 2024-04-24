using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public AudioSource jumpAudioSource;
    public PlatformBounce bouncePlatform;

    public Tilemap ptilemap;

    public Tilemap btilemap;
    public Transform spawnBounce;

    public Transform spawnBounce2;

    public GameObject bossStarter;

    public GameObject defCam;

    public GameObject boss;

    public GameObject boss2;

    public GameObject boss3;

    public float bossIteration = -1f;
    [SerializeField] float runSpeed = 10f; //float to scale the run speed
    [SerializeField] float jumpSpeed = 5f; //float to set the jump speed
    [SerializeField] float climbSpeed = 5f; //float to scale the climb speed
    [SerializeField] Vector2 deathKick = new Vector2 (10f, 10f); //sets vector for death animation
    [SerializeField] GameObject bullet; //object for bullet
    [SerializeField] Transform gun; //transform from where the bullet is being instantiated

    public Vector2 moveInput; //variable that will store the input that is being given
    Rigidbody2D myRigidbody; //rigidBody variable that represents the character
    Animator myAnimator; //Animator variable that represents the animator
    CapsuleCollider2D myBodyCollider; //var that represents the player collision hurtbox
    BoxCollider2D myFeetCollider; //var that represents the players feet, prevents wall jumping
    float gravityScaleAtStart; //var that will store the gravity constant used
    public bool isAlive = true; //is character alive

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>(); //on init, set variable to the rigidbody component
        myAnimator = GetComponent<Animator>(); //on init, set var to the Animator component, "catch" reference
        myBodyCollider = GetComponent<CapsuleCollider2D>(); //on init, catch capsulecollider ref 
        myFeetCollider = GetComponent<BoxCollider2D>(); //on init, catch boxcollider ref
        gravityScaleAtStart = myRigidbody.gravityScale; //init gravityScale to current player gravity   
        defCam.gameObject.SetActive(true);
        
    }

    void Update()
    {
        if (!isAlive) {return;} //checks if player is alive
        if (GlobalVariables.instance.isInTextInput) {return;}
        Run(); //on every update time, run method Run()
        FlipSprite(); //on every update time, flip sprite along y-axis if running the other way
        ClimbLadder(); //on every update time, climb the ladder
        Die(); //calls the die function
        LevelLoop();
    }

    void OnFire(InputValue value) //method called when bullet is fired
    {
        if (GlobalVariables.instance.isInTextInput) {return;}
        if (!isAlive) {return;}
        var bullClone = GameObject.Find("Bullet2(Clone)");
        if(bullClone){
            Destroy(bullClone);
        }
        myAnimator.SetTrigger("Shoot");
        Instantiate(bullet, gun.position, transform.rotation);
    }
 
    void OnMove(InputValue value) //method to store the input into the Vector2 variable we created, the log it
    {
        if (!isAlive) {return;} //checks if player is alive
        moveInput = value.Get<Vector2>(); //gets the value, stores it into moveInput
    }

    void OnJump(InputValue value)
    {
        if (GlobalVariables.instance.isInTextInput) {return;}
        if (!isAlive) {return;} //checks if player is alive
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Bullets"))) {return;} //if not touching ground, dont proceed
        if(value.isPressed) //is the jump button pressed?
        {
            myRigidbody.velocity += new Vector2 (0f, jumpSpeed);
            // play the jump sound effect from the JumpSFX game object
            jumpAudioSource.Play();
        }
    }

    void OnHamAttack(InputValue value){
        if (GlobalVariables.instance.isInTextInput) {return;}
        if(!isAlive){
            return;
        }
        myAnimator.SetTrigger("Attack");
    }


    void Run() //lets the player move horizontally
    {
        
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidbody.velocity.y); //sets x and y movement speed, for x move runSpeed times faster, for y just keep same velocity you currently have
        myRigidbody.velocity = playerVelocity; //set velocity of player to the playerVelocity value
        
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; //checks if player is moving or not
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed); //sets the isRunning bool based on if player is moving or not
    }

    void FlipSprite() //flips the player sprite
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; //checks if player is moving or not
        if (playerHasHorizontalSpeed) //if player is moving, transform
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f); //gets sign based on x velocity, uses it to transform the scale 
        }
    }

    void ClimbLadder() //climbs the ladder. Added note, set as trigger to prevent collision with player
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) //if not touching ground, dont proceed
        {
            myRigidbody.gravityScale = gravityScaleAtStart; //set gravity back to init gravity
            myAnimator.SetBool("isClimbing", false); //isClimbing set to false
            return;
        }
        
        Vector2 climbVelocity = new Vector2 (myRigidbody.velocity.x, moveInput.y * climbSpeed); //sets x and y movement speed, for x move runSpeed times faster, for y just keep same velocity you currently have
        myRigidbody.velocity = climbVelocity; //set velocity of player to the playerVelocity value
        myRigidbody.gravityScale = 0f; //set gravity to 0 to prevent falling

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon; //checks if player is moving or not
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed); //sets isClimbing to if player is moving up ladder or not

    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))) //is player touching enemy?
        {
            isAlive = false; //no longer alive
            myAnimator.SetTrigger("Dying"); //trigger dying state
            myBodyCollider.enabled = false;
            myFeetCollider.enabled = false;
            myRigidbody.velocity = deathKick; //trigger death animation
            
            
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    void LevelLoop()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("LevelExit")))
        {
            isAlive = false;
            FindObjectOfType<GameSession>().ProcessLevelLoop();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
       // Destroy(gameObject);
       if(other.tag == "bossStart"){
            
            myAnimator.SetBool("isRunning", false);
            myRigidbody.velocity = new Vector2(0f, 0f);
            moveInput = new Vector2(0f, 0f);
            //StartCoroutine(waiter());
            //Destroy(other.gameObject);
            //System.Threading.Thread.Sleep(10000);
            //bossStarter.SetActive(false);
       }
       if(other.tag == "Anvil")
        {
            Debug.Log("we are touching the anvil");
            GlobalVariables.instance.isInAnvilArea = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Jump"))
        {
            Debug.Log("Collided with object tagged: Jump");
            if(Input.GetKeyDown(KeyCode.F))
            {
                Destroy(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Anvil")
        {
            GlobalVariables.instance.isInAnvilArea = false;
        }
    }

    public void createBounce(){

        //var pos = Camera.main.ScreenToWorldPoint(spawnBounce);
        //pos.z = 0f;

        if(bossIteration == 1){
        Vector3Int cell = ptilemap.WorldToCell(spawnBounce.position);
 
        ptilemap.SetTile(bouncePlatform.cell0 + cell, bouncePlatform.tile0);
        ptilemap.SetTile(bouncePlatform.cell1 + cell, bouncePlatform.tile1);
        ptilemap.SetTile(bouncePlatform.cell2 + cell, bouncePlatform.tile2);
        btilemap.SetTile(bouncePlatform.cell3 + cell, bouncePlatform.tile3);
        }
        else if (bossIteration == 3){
        Vector3Int cell = ptilemap.WorldToCell(spawnBounce2.position);
        ptilemap.SetTile(bouncePlatform.cell0 + cell, bouncePlatform.tile0);
        ptilemap.SetTile(bouncePlatform.cell1 + cell, bouncePlatform.tile1);
        ptilemap.SetTile(bouncePlatform.cell2 + cell, bouncePlatform.tile2);
        btilemap.SetTile(bouncePlatform.cell3 + cell, bouncePlatform.tile3);
        }


    }

    IEnumerator waiter()
{
    bossStarter.SetActive(true);
    
    isAlive = false;

    //So player doesn't move during view of scene
    yield return new WaitForSeconds(3);
    isAlive = true;
    bossStarter.SetActive(false);
}

    IEnumerator waiter2()
{
    if(bossIteration == 1){ 
    bossStarter.SetActive(true);
    isAlive = false;
    //So player doesn't move during view of scene
    yield return new WaitForSeconds(3);
    isAlive = true;
    bossStarter.SetActive(false);
    }
}


}
