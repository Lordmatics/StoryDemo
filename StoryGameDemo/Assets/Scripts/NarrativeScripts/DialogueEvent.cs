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
    public string lyricString;
    public int targetBranchNum;
    public int possibleAnswersNum;
}
