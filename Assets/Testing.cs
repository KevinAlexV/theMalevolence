using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class Testing : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        GameManager.manager.testing = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        /*
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }

        Debug.Log($"Active scene is {SceneManager.GetActiveScene().name} and scenes enabled include {EditorScenes[1].ToString()}");
        */

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            Debug.Log("Quitting...");
            Application.Quit();
        }
        else
        { 
            if (GameManager.manager.phase == Enums.GameplayPhase.Planning)
            {
                GameManager.manager.EndPlanning();
            }
            else if (GameManager.manager.phase == Enums.GameplayPhase.Resolve)
            {

            }
        }
    }
}
