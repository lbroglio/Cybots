using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // The players running speed
    public float Speed = 10.0f;

    // How long a cybot freeze last
    public float FreezeDuration = 1.0f;

    // How long after being unfrozen the player is "invincible"
    public float InvincibilityTime = 1.0f;

    // Mouse sensitivity
    public float xSensitivity = 1000f;
    public float ySensitivity = 200f;

    // Jumping 
    public Vector3 Jump = new Vector3(0, 2f, 0);
    public float JumpForce = 2.0f;
    private bool grounded = true;

    // Freezing
    private bool isFrozen = false;
    private float timeFrozen = 0.0f;
    private float timeUnFrozen = 0.0f;

    // Invinciblity to freezing
    private bool isInvince = false;
    private float invinceTime = 0.0f;

    public bool getFrozen()
    {
        return isFrozen;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        // Spawning Logic

        // Get the map
        MapBuilder map = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();

        // Calculate spawn coords
        Vector3 spawnCords = map.MazeLocToCoords(map.ChooseRandomMazePointInRow(0));

        gameObject.transform.position = spawnCords;
        
    }

    void OnCollisionStay(Collision collisionInfo)
    {

        if(collisionInfo.gameObject.name == "Floor")
        {
            grounded = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Movement

        // Rotation -- WIP

        // Get input
        float xRot = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        float yRot = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;

        // Rotate player body (x rotation)
        gameObject.transform.Rotate(Vector3.up * xRot);

        // Rotate camera(y rotation)
        Transform camera = gameObject.transform.GetChild(0);

        camera.Rotate(Vector3.left * yRot);

        // Handle unfreezing and frozen effects
        if (isFrozen)
        { 

            // Check if they have been frozen for the full time
            if(Time.time > timeFrozen + FreezeDuration)
            {   // Remove ice effect
                transform.Find("Canvas/Image").gameObject.SetActive(false);
                
                isFrozen = false;
                timeUnFrozen = Time.time;
            }
        }

        // If the player is frozen they can't move

        if (!isFrozen)
        {
            // Walking
            if (Input.GetKey(KeyCode.W))
            {
                gameObject.transform.position += transform.rotation * Vector3.forward * Speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                gameObject.transform.position += transform.rotation * Vector3.back * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.position += transform.rotation * Vector3.right * Speed * Time.deltaTime;

            }
            else if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.position += transform.rotation * Vector3.left * Speed * Time.deltaTime;
            }


            // Jumping
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                grounded = false;
                GetComponent<Rigidbody>().AddForce(Jump * JumpForce, ForceMode.Impulse);
            }
        }

        // Disable invisibility if its time is up
        if(isInvince && Time.time - invinceTime > 30.0f)
        {
            Debug.Log("Invincibiltiy Off");
            isInvince = false;
        }



    }

    /// <summary>
    /// Handle logic for when  a player is hit by a Cybot's Freeze eyes
    /// </summary>
    public void Freeze()
    {
        // If the player isn't within the window of invincibility after being frozen
        if(Time.time > timeUnFrozen + InvincibilityTime && !isFrozen && !isInvince)
        {
            timeFrozen = Time.time;
            isFrozen = true;
            transform.Find("Canvas/Image").gameObject.SetActive(true);

        }
    }

    // Handle collision with power ups
    private void OnCollisionEnter(Collision collision)
    {
        GameObject objHit = collision.gameObject;
        GameController gameCon = GameObject.Find("GameController").GetComponent<GameController>();


        // If a light power up was hit
        if (objHit.name == "LightPowerUp(Clone)")
        {
            // Activate light function in the game controller
            gameCon.lightPowerUp(objHit);
        }
        // If an invincibility power up was hit
        else if(objHit.name == "InvincPowerUp(Clone)")
        {
            Debug.Log("Invincibiltiy On");
            // Set invisibility variables
            isInvince = true;
            invinceTime = Time.time;

            // Destory and replace power up
            Destroy(objHit);
            gameCon.spawnInvincPowerUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Switch to win screen
        SceneManager.LoadScene("Scenes/EndScene");
    }
}
