using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestActEvent
{
    public List<TestAct> actConfig;
}

[System.Serializable]
public struct TestAct
{
    //public Mood lyricMood;
    public int branchIndex;
    public int numOfAnswers;
    public List<NarrativeData> narrativeData; // File to load + Answer Button String
}

public class TestRealm : MonoBehaviour {

    [SerializeField]
    TestActEvent currentActConfig;

	// Use this for initialization
	void Start ()
    {
        //currentActConfig = TextFactory.TextAssembly.RunTextFactory<TestActEvent>("Test/TestAct");
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
