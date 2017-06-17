using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextFactory;

[AddComponentMenu("Scripts/NarrativeScripts/TestFileRead")]
public class TestFileRead : MonoBehaviour
{
    private void Start()
    {
        DialogueEvent dialogue = TextAssembly.RunTextFactoryForFile("Test/TestTextFile");
        for (int i = 0; i < dialogue.dialogues.Count; i++)
        {
            Debug.Log(dialogue.dialogues[i].lyricString);
        }
    }

}
