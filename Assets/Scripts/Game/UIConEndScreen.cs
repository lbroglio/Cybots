using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controller for the UI after winning the game
/// </summary>
public class UIConEndScreen : MonoBehaviour
{
    // When the restart button is pressed go back to main scene
    public void onPressRestart()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    // Chnage the number of active cybots
    public void onChange()
    {
        // Get the dropdown
        TMP_Dropdown dropdown = GameObject.Find("Canvas").transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();

        // Set number of cybots
        GameConfig.numCybots = dropdown.value + 2;

        Debug.Log(dropdown.value + 2);
    }




    // Start is called before the first frame update
    void Start()
    {
        // Setup dropdown
        TMP_Dropdown dropdown = GameObject.Find("Canvas").transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();


        dropdown.value = GameConfig.numCybots - 2;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
