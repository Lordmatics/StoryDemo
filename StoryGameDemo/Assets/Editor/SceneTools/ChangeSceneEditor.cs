using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ChangeSceneEditor : Editor
{

    [MenuItem("OpenScene/Main")]
    public static void OpenMainScene()
    {
        OpenScene("Main");
    }

    static void OpenScene(string name)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
        }
    }
}
