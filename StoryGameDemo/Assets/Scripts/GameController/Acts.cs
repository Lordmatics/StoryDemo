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
    public int maxBranches;
    public List<NarrativeData> narrativeData;
}

public abstract class Act
{
    protected int branchCount;

    [SerializeField]
    protected List<Branch> branches = new List<Branch>();

    public abstract int GetBranchCount();

    public abstract void IncrementBranchCount();

    public abstract void DecrementBranchCount(int howMuch);

    public abstract string GetClassName();

    public abstract List<Branch> GetBranches();
}

[System.Serializable]
public class Act_01 : Act
{
    public override int GetBranchCount()
    {
        return branchCount;
    }

    public override void IncrementBranchCount()
    {
        branchCount++;
    }

    public override void DecrementBranchCount(int howMuch)
    {
        branchCount -= howMuch;
    }

    public override string GetClassName()
    {
        return "Act_01";
    }

    public override List<Branch> GetBranches()
    {
        return branches;
    }
}

[System.Serializable]
public class Act_02 : Act
{

    public override int GetBranchCount()
    {
        return branchCount;
    }

    public override void IncrementBranchCount()
    {
        branchCount++;
    }

    public override void DecrementBranchCount(int howMuch)
    {
        branchCount -= howMuch;
    }

    public override string GetClassName()
    {
        return "Act_02";
    }

    public override List<Branch> GetBranches()
    {
        return branches;
    }
}

public class Acts : MonoBehaviour
{

    public static Acts instance = null;

    [SerializeField]
    private bool bShouldPersist = false;

    public Act_01 act_01;

    public Act_02 act_02;

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
        allActs.Add(act_01);
        allActs.Add(act_02);
    }
    public Act GetCurrentAct()
    {
        return allActs[currentAct - 1];
    }
}
