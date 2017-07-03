using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvent
{
    public List<Dialogues> dialogues;
}

public struct Dialogues
{
    //public Mood lyricMood;
    public string sentence;
    public int currentBranchNum;
    public int targetBranchNum;
    public int possibleAnswersNum;
    public bool bIsVideo;
    public string videoPath;
}
