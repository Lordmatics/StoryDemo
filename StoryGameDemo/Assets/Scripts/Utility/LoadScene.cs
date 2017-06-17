using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Utility/LoadScene")]
public class LoadScene : MonoBehaviour
{

    public enum ScenesToLoad
    {
        E_MainMenu,
        E_Game,
        E_Quit
    }

    [Header("Make sure the build settings match the order of this enum")]
    [Tooltip("Main Menu = 0 , Game = 1 , Quit = 2")]
    public ScenesToLoad sceneToLoad;

    public void LoadThisScene()
    {
        switch(sceneToLoad)
        {
            case ScenesToLoad.E_MainMenu:
                SceneManager.LoadScene((int)ScenesToLoad.E_MainMenu);
                break;
            case ScenesToLoad.E_Game:
                SceneManager.LoadScene((int)ScenesToLoad.E_Game);
                break;
            case ScenesToLoad.E_Quit:
                Application.Quit();
                break;
        }
    }
}
