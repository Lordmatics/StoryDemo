using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrativeData
{
    public string filePath;
    public string answerData;
}

[System.Serializable]
public class Branch
{
    public int branchIndex;
    public int numOfAnswers;
    public List<NarrativeData> narrativeData;
}

[System.Serializable]
public class ActEvent
{
    public List<Branch> branchInfo;
}

[System.Serializable]
public class Act
{
    [SerializeField]
    private int branchCount;
    [SerializeField]
    private string name;

    [SerializeField]
    private ActEvent branches;

    public Act(string name)
    {
        this.name = name;
        branches = TextFactory.TextAssembly.RunTextFactory<ActEvent>("Test/" + name);
        //Debug.Log("Branches Assigned");
    }

    public int GetBranchCount()
    {
        return branchCount;
    }

    public void IncrementBranchCount()
    {
        branchCount++;
    }

    public void SetBranchCount(int newBranchCount)
    {
        branchCount = newBranchCount;
    }

    public void DecrementBranchCount(int howMuch)
    {
        branchCount -= howMuch;
    }

    public string GetClassName()
    {
        return name;
    }

    public List<Branch> GetBranches()
    {
        return branches.branchInfo;
    }
}

public class Acts : MonoBehaviour
{

    public static Acts instance = null;

    [SerializeField]
    private bool bShouldPersist = false;

    //public Act_01 act_01;

    //public Act_02 act_02;

    //public List<Act> allActs = new List<Act>();

    public List<Act> allActs = new List<Act>();

    [SerializeField]
    private int currentAct = 2;

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces the singleton pattern
            Destroy(gameObject);

        if(bShouldPersist)
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Act act_01 = new Act("Act_01");
        Act act_02 = new Act("Act_02");
        allActs.Add(act_01);
        allActs.Add(act_02);
    }

    public Act GetCurrentAct()
    {
        return allActs[currentAct - 1];
    }
}
