using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ChangeSceneEditor : Editor
{

    [MenuItem("OpenScene/MainMenu")]
    public static void OpenMainMenuScene()
    {
        OpenScene("MainMenu");
    }

    [MenuItem("OpenScene/Game")]
    public static void OpenMainScene()
    {
        OpenScene("Main");
    }

    [MenuItem("OpenScene/TestRealm")]
    public static void OpenTestRealm()
    {
        OpenScene("TestRealm");
    }

    static void OpenScene(string name)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
        }
    }
}
