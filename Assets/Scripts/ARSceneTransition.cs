using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneTransition : MonoBehaviour
{
    public TMP_Text originLatLng;
    public TMP_Text destinationLatLng;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteKey("my origin");
        PlayerPrefs.DeleteKey("my destination");
    }

    // Update is called once per frame
    public void Launch()
    {
        if (originLatLng.text == "" ||  destinationLatLng.text == "")
        {
            Debug.LogError("Insufficient parameter.");
            return;
        } 

        PlayerPrefs.SetString("my origin", originLatLng.text);
        PlayerPrefs.SetString("my destination", destinationLatLng.text);
        
        GoToARScene();
    }

    void GoToARScene()
    {
        // Get the index of the current scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene by incrementing the index
        SceneManager.LoadScene(currentSceneIndex + 1);

        // Alternatively, you can specify the scene name instead of the index
        // SceneManager.LoadScene("NextScene");
    }
}

