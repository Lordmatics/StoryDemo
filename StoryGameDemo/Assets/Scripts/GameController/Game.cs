using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[AddComponentMenu("Scripts/GameController/Game")]
public class Game : MonoBehaviour
{
    public static bool bCanProceed = true;

    private DialogueEvent currentEvent;

    [SerializeField]
    private Text rpgText;

    [SerializeField]
    private int currentStepIndex = 0;

    [SerializeField]
    private GameObject boxToDuplicate;

    [SerializeField]
    private RectTransform parentForScroll;

    [SerializeField]
    private RectTransform parentForAnswers;

    [SerializeField]
    private List<RectTransform> previousMessages = new List<RectTransform>();

    [SerializeField]
    private List<GameObject> buttonList = new List<GameObject>();

    [SerializeField]
    private float lerpSpeed = 5.0f;

    [SerializeField]
    private float scrollSpeed = 5.0f;

    /** See Scroller script*/
    public static Action OnScrollFinished;

    /** See CanProceedFeedback script*/
    public static Action<bool> OnProceedChanged;

    private void Awake()
    {
        rpgText = GameObject.Find("RPG_Message").GetComponent<Text>();
        boxToDuplicate = GameObject.Find("TextBGPanel");
        parentForScroll = GameObject.Find("ScrollZone").GetComponent<RectTransform>();
        parentForAnswers = GameObject.Find("AnswerButtonParent").GetComponent<RectTransform>();
    }

    private void Start()
    {
        currentEvent = TextFactory.TextAssembly.RunTextFactoryForFile(FileContainer.ActTwo_01);
        rpgText.text = currentEvent.dialogues[currentStepIndex++].lyricString;
        //if(currentEvent.dialogues.Count <= 1)
        //{
        //    bCanProceed = false;
        //    if (OnProceedChanged != null)
        //        OnProceedChanged(bCanProceed);
        //}
    }

    public Text curActText;
    public Text curBranchNum;
    public Text curBranchIndex;
    public Text curBranchTotal;
    public Text curData;

    void DebugHelper()
    {
        Act currentAct = Acts.instance.GetCurrentAct();
        Branch currentBranch = currentAct.GetBranches()[currentAct.GetBranchCount()];

        curActText.text = "Act Name: " + currentAct.GetClassName();
        curBranchNum.text = "Branch Count: " + currentAct.GetBranchCount().ToString();
        curBranchIndex.text = "Branch Index: " + currentBranch.branchIndex;
        curBranchTotal.text = "Branch Total: " + currentBranch.maxBranches;
        curData.text = "NarrativeData: " + currentBranch.narrativeData.Count;

    }

    private void Update()
    {
        DebugHelper();
        if (previousMessages.Count <= 0) return;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // Move Scroll down
            //parentForScroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top)
            Vector2 leftBot = parentForScroll.offsetMin;
            Vector2 rightTop = parentForScroll.offsetMax;
            leftBot.y -= 400.0f * Time.deltaTime;
            rightTop.y -= 400.0f * Time.deltaTime;
            Debug.Log(leftBot.y + ": LeftbotY" + " And " + rightTop.y + " : RightTopY");

            if (previousMessages.Count < 3)
            {
                leftBot.y = Mathf.Clamp(leftBot.y, 0.0f, 620.0f);
                rightTop.y = Mathf.Clamp(rightTop.y, 0.0f, 270.0f);
            }
            else
            {
                // This algorthm took forever to calculate xD
                leftBot.y = Mathf.Clamp(leftBot.y, -1 * (((previousMessages.Count - 2) * 330.0f) - 270.0f), 0.0f);
                rightTop.y = Mathf.Clamp(rightTop.y, -1 * (((previousMessages.Count - 2) * 330.0f) - 270.0f), 0.0f);
            }

            parentForScroll.offsetMin = leftBot;
            parentForScroll.offsetMax = rightTop;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector2 leftBot = parentForScroll.offsetMin;
            Vector2 rightTop = parentForScroll.offsetMax;
            leftBot.y += 400.0f * Time.deltaTime;
            rightTop.y += 400.0f * Time.deltaTime;


            if (previousMessages.Count < 3)
            {
                leftBot.y = Mathf.Clamp(leftBot.y, 0.0f, 0.0f);
                rightTop.y = Mathf.Clamp(rightTop.y, 0.0f, 0.0f);
            }
            else
            {
                leftBot.y = Mathf.Clamp(leftBot.y, leftBot.y, 0.0f);
                rightTop.y = Mathf.Clamp(rightTop.y, rightTop.y, 0.0f);
            }
            parentForScroll.offsetMin = leftBot;
            parentForScroll.offsetMax = rightTop;
        }
    }

    #region OnTextPressed
    void IncrementStepIndex()
    {
        currentStepIndex++;
        if (currentStepIndex >= currentEvent.dialogues.Count)
        {
            SetCanProceed(false);
        }
    }
    #endregion

    void SpawnAnswerButton(int index, string buttonText)
    {
        Debug.Log("SpawnButton");
        GameObject button = (GameObject)Instantiate(Resources.Load("Prefabs/AnswerButton"));
        button.transform.SetParent(parentForAnswers);
        ButtonAnswer script = button.GetComponent<ButtonAnswer>() ? button.GetComponent<ButtonAnswer>() : button.AddComponent<ButtonAnswer>();
        script.SetOptionIndex(index);
        buttonList.Add(button);
        Text textButton = button.GetComponentInChildren<Text>();
        textButton.text = buttonText.ToString();
    }

    public void DestroyButtons()
    {
        foreach (GameObject obj in buttonList)
        {
            Destroy(obj);
        }
        buttonList.Clear();
    }

    // Answer Button index
    public void BranchNarrative(int index)
    {
        // Get Act - I.E Act 2
        Act currentAct = Acts.instance.GetCurrentAct();
        // Get Number of Branches
        int curBranch = currentAct.GetBranchCount();
        // Get the data at that branch
        Branch currentBranch = currentAct.GetBranches()[curBranch];
        // Get the max options for this branch
        int numOfAnswers = currentBranch.maxBranches;
        // Based on the index of the answer button
        // Select the right narrative to branch to

        int offset = currentEvent.dialogues[currentStepIndex - 1].delayToMainBranch;

        string filePath = currentBranch.narrativeData[index + offset].filePath;

        // Reset to start of new event
        currentStepIndex = 0;

        // Increment Branch Count for Current Act
        currentAct.IncrementBranchCount();

        // Need a way to track the branching - Above might do it
        currentEvent = TextFactory.TextAssembly.RunTextFactoryForFile(filePath);

        // Basically, if there is branching narrative, that eventually goes back to "main branch"
        // being the branch that ultimately progresses through the story
        // Find out how many side narratives there are until you meet up
        int delayToMainBranch = currentEvent.dialogues[currentStepIndex].delayToMainBranch;

        // Rewind to main branch, then proceed
        //currentAct.IncrementBranchCount();
        currentAct.DecrementBranchCount(delayToMainBranch);
        
        

        // Enable clicking
        SetCanProceed(true);

        // Load next dialogue
        ProceedThroughDialogueEvent();
    }


    public void ProceedThroughDialogueEvent()
    {
        if (!bCanProceed)
        {
            Debug.Log("bCanProceed == false");
            return;
        }
        if (currentStepIndex >= currentEvent.dialogues.Count)
        {
            // Reached end of current branch
            // Get option count
            int numberOfPotentialAnswers = currentEvent.dialogues[currentStepIndex - 1].branchNumber;
            Debug.Log("Step INdex: " + (currentStepIndex - 1).ToString());
            string buttonText = "";
            for (int i = 0; i < numberOfPotentialAnswers; i++)
            {
                // Get Act - I.E Act 2
                Act currentAct = Acts.instance.GetCurrentAct();
                // Get Number of Branches
                int curBranch = currentAct.GetBranchCount();
                Debug.Log("Branch Count: " + curBranch);
                // Get the data at that branch
                Branch currentBranch = currentAct.GetBranches()[curBranch];
                // Get the data for the button
                Debug.Log("Data" + currentBranch.narrativeData[i].answerData);
                int offset = currentEvent.dialogues[currentStepIndex - 1].delayToMainBranch;

                buttonText = currentBranch.narrativeData[i + offset].answerData;
                // Pass it all through to the button
                SpawnAnswerButton(i, buttonText); 
            }
            SetCanProceed(false);
            Debug.Log("Index Out of Bounds");
            return;
        }
        ScrollUp();

        rpgText.text = currentEvent.dialogues[currentStepIndex].lyricString;

        //switch (currentEvent.dialogues[currentStepIndex].branchNumber)
        //{
        //    // Reset step index.
        //    // Load Branching Narrative
        //    // Assign new Text

        //    // Continue current branch
        //    case 0:
        //        rpgText.text = currentEvent.dialogues[currentStepIndex].lyricString;
        //        Debug.Log("Case 0 Activated");
        //        break;
        //    // Load new branch
        //    case 1:
        //        break;
        //    case 2:
        //        break;
        //    default:
        //        break;
        //}
        bCanProceed = false;
        IncrementStepIndex();
    }

    void ScrollUp()
    {
        // Duplicate contents.
        GameObject message = (GameObject)Instantiate(boxToDuplicate);
        // Make sure only the original text box can proceed the dialogue
        Destroy(message.GetComponent<TextBoxPressed>());
        RectTransform rect = message.GetComponent<RectTransform>();
        // Set parent to enable scrolling manually and apply the mask
        message.transform.SetParent(parentForScroll);
        // Default anchor position, since the scroll maths, is + 0.35f on the anchors
        rect.anchorMin = new Vector2(0.0f, -0.35f);
        rect.anchorMax = new Vector2(1.0f, 0.0f);
        // Add to scroll container
        previousMessages.Add(rect);
        // Lerp all contents up
        // Doing it this way, makes them move in unison
        // As opposed to waiting for each box to finish
        foreach (RectTransform t in previousMessages)
        {
            Scroller scroll = t.GetComponent<Scroller>();
            if (scroll != null)
            {
                StartCoroutine(scroll.InitiateScroll());
            }
        }
    }

    void SetCanProceed(bool canProceed)
    {
        bCanProceed = canProceed;
        if (OnProceedChanged != null)
            OnProceedChanged(bCanProceed);
    }
}
