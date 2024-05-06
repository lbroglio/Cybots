using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public List<Cybot> typesToSpawn;

    private List<Cybot> SpawnedCybots;

    private GameObject player;

    private MapBuilder map;

    public GameObject dirLight;

    /// <summary>
    /// Gametime when a light power up was activated
    /// </summary>
    private float lightTime = 0;

    // Track how many cybots are within capture distance of the player
    private int cybotsCloseToPlayer = 0;

    // Track if any cybots are chasing the player
    private int chasesOccuring = 0;

    // Methods for cybots to say if they are chasing
    public void amChasing()
    {
        chasesOccuring++;
    }

    public void stopChasing()
    {
        chasesOccuring--;
    }

    // Get if the player is being chased
    public bool chaseOccuring()
    {
        if(chasesOccuring > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Spawn a power up which turns on the sunlight
    /// </summary>
    public void spawnLightPowerUp()
    {
        // Choose a random point for this sentry to spawn this power up
        int pointX = Random.Range(0, map.MazeWidth);
        int pointY = Random.Range(0, map.MazeHeight);
        Vector3 spawnLoc = map.SpaceLocToCoords(new System.Drawing.Point(pointX, pointY));

        // Spawn the power up
        GameObject toSpawn = Resources.Load("Prefabs/LightPowerUp") as GameObject;
        Instantiate(toSpawn, new Vector3(spawnLoc.x, 1, spawnLoc.z), Quaternion.Euler(30, 0, 0));
    }


    /// <summary>
    /// Spawn a power up which gives the player 30 seconds on immunity to freeze rays
    /// </summary>
    public void spawnInvincPowerUp()
    {
        // Choose a random point for this sentry to spawn this power up
        int pointX = Random.Range(0, map.MazeWidth);
        int pointY = Random.Range(0, map.MazeHeight);
        Vector3 spawnLoc = map.SpaceLocToCoords(new System.Drawing.Point(pointX, pointY));

        // Spawn the power up
        GameObject toSpawn = Resources.Load("Prefabs/InvincPowerUp") as GameObject;
        Instantiate(toSpawn, new Vector3(spawnLoc.x, 1, spawnLoc.z), Quaternion.Euler(30, 0, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();

        UnityEngine.Debug.ClearDeveloperConsole();
        // Add Player
        GameObject playerObj = Resources.Load("Prefabs/FirstPersonRig") as GameObject;
        player = Instantiate(playerObj);

        SpawnedCybots = new List<Cybot>();
        // Add Cybots
        for(int i=0; i < GameConfig.numCybots; i++)
        {
            SpawnedCybots.Add(Instantiate(typesToSpawn[i]));

        }

        spawnLightPowerUp();
        spawnInvincPowerUp();

    }

    // Update is called once per frame
    void Update()
    {
        // Set the number of cybots that are close to the player
        cybotsCloseToPlayer = 0;
        foreach(Cybot c in SpawnedCybots)
        {
            if(Vector3.Distance(c.gameObject.transform.position, player.transform.position) < 1.5f)
            {
                cybotsCloseToPlayer++;
            }
        }

        // If two or more cybots are near player; lose and restart
        if(cybotsCloseToPlayer >= 2)
        {
            SceneManager.LoadScene("Scenes/SampleScene");
        }


        // If the light power up has been actiavted for 10 seconds turn it off
        if(dirLight.activeInHierarchy && Time.time - lightTime > 10.0f)
        {
            dirLight.SetActive(false);
        }
    }

    // Get how many cybots are near the player
    public int cybotsNearPlayer()
    {
        return cybotsCloseToPlayer;
    }

    /// <summary>
    /// Handle activating a light power up
    /// </summary>
    public void lightPowerUp(GameObject powerUpHit)
    {
        lightTime = Time.time;
        //Turn on the directional light
        dirLight.SetActive(true);

        // Delete and replace this power up
        Destroy(powerUpHit);
        spawnLightPowerUp();

    }
}
