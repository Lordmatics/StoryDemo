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

    [SerializeField]
    private int targetBranchNumber = 1;

    [SerializeField]
    private int eventTarget = 0;

    private void Awake()
    {

        rpgText = GameObject.Find("RPG_Message").GetComponent<Text>();
        boxToDuplicate = GameObject.Find("TextBGPanel");
        parentForScroll = GameObject.Find("ScrollZone").GetComponent<RectTransform>();
        parentForAnswers = GameObject.Find("AnswerButtonParent").GetComponent<RectTransform>();
    }

    private void Start()
    {
        currentEvent = TextFactory.TextAssembly.RunTextFactory<DialogueEvent>(FileContainer.ActTwo_01);
        rpgText.text = currentEvent.dialogues[currentStepIndex++].sentence;
        Debug.Log("Count" + currentEvent.dialogues.Count + " stepindex " + currentStepIndex);
        eventTarget = currentEvent.dialogues[currentStepIndex - 1].targetBranchNum;
    }

    #region DEBUG
    public Text curActText;
    public Text curBranchNum;
    public Text curBranchIndex;
    public Text curBranchTotal;
    public Text curData;
    public Text target;
    public Text eventTargetText;
    #endregion
    void DebugHelper()
    {
        
        Act currentAct = Acts.instance.GetCurrentAct();
        Branch currentBranch = currentAct.GetBranches()[currentAct.GetBranchCount()];

        curActText.text = "Act Name: " + currentAct.GetClassName();
        curBranchNum.text = "Branch Count: " + currentAct.GetBranchCount().ToString();
        curBranchIndex.text = "Branch Index: " + currentBranch.branchIndex;
        curBranchTotal.text = "Branch Total: " + currentBranch.numOfAnswers;
        curData.text = "NarrativeData: " + currentBranch.narrativeData.Count;
        target.text = "Target Branch: " + targetBranchNumber;
        eventTargetText.text = "EV_Target: " + eventTarget;
    }

    private void Update()
    {
        DebugHelper();
        ScrollChat();
    }

    void ScrollChat()
    {
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
        bool bVideo = currentEvent.dialogues[currentStepIndex++].bIsVideo;
        if (currentStepIndex >= currentEvent.dialogues.Count && !bVideo)
        {
            SetCanProceed(false);
        }
    }
    #endregion

    void SpawnAnswerButton(int index, string buttonText)
    {
        //Debug.Log("SpawnButton");
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
        int prevBranchIndex = currentAct.GetBranchCount();
        
        // Get the data at that branch
        Branch previousBranch = currentAct.GetBranches()[prevBranchIndex];
        // Get the max options for this branch
        int numOfAnswers = previousBranch.numOfAnswers;
        // Based on the index of the answer button
        // Select the right narrative to branch to

        string filePath = previousBranch.narrativeData[index].filePath;

        // Reset to start of new event
        currentStepIndex = 0;

        // Linear Progression on main branch - This will be auto adjusted if a side branch is taken
        currentAct.IncrementBranchCount();
        targetBranchNumber++;

        // Need a way to track the branching - Above might do it
        currentEvent = TextFactory.TextAssembly.RunTextFactoryForFile(filePath);

        // Enable clicking
        SetCanProceed(true);

        // Makes video autoplay
        if (currentEvent.dialogues[currentStepIndex].bIsVideo)
        {
            ProceedThroughDialogueEvent();
            //Debug.Log("Proceed on a video");
        }


        // Load next dialogue
        ProceedThroughDialogueEvent();
    }


    Action VideoCallback;

    private void OnEnable()
    {
        //VideoCallback += ProceedThroughDialogueEvent;
        VideoCallback += CreateAnswers;
    }

    private void OnDisable()
    {
        //VideoCallback -= ProceedThroughDialogueEvent;
        VideoCallback -= CreateAnswers;
    }

    void CreateAnswers()
    {
        // Prevent this from being repeated maybe?
        Act currentAct = Acts.instance.GetCurrentAct();
        int dialogueIndex = currentStepIndex - 1;
        //Debug.Log("DialogueIndex: " + dialogueIndex);
        int numberOfPotentialAnswers = currentEvent.dialogues[dialogueIndex].possibleAnswersNum;
        string buttonText = "";
        int curBranch = currentAct.GetBranchCount();
        // Cache current branch
        Branch currentBranch = currentAct.GetBranches()[curBranch];
        for (int i = 0; i < numberOfPotentialAnswers; i++)
        {
            //Debug.Log("Data" + currentBranch.narrativeData[i].answerData);
            // Extract button data from current branch config
            if (i >= currentBranch.narrativeData.Count)
            {
                //Debug.Log("No More Narrative Data At Branch: " + curBranch);
                return;
            }
            buttonText = currentBranch.narrativeData[i].answerData;
            SpawnAnswerButton(i, buttonText);
        }
    }

    public void ProceedThroughDialogueEvent()
    {
        //Debug.Log("VideoCallback Ran");
        if (!bCanProceed)
        {
            //Debug.Log("bCanProceed == false");
            return;
        }
        // Cache current act
        Act currentAct = Acts.instance.GetCurrentAct();
        Debug.Log("-----------------CurrentStepIndex:" + currentStepIndex.ToString() + " Count: " + currentEvent.dialogues.Count.ToString());
        if (currentStepIndex >= currentEvent.dialogues.Count)
        {
            // Need a constraint here, if there are no more narrative entries for this act

            // Reached end of current branch
            // Spawn Answer buttons
            //int numberOfPotentialAnswers = currentEvent.dialogues[currentStepIndex - 1].possibleAnswersNum;
            //string buttonText = "";
            //int curBranch = currentAct.GetBranchCount();
            // Cache current branch
            //Branch currentBranch = currentAct.GetBranches()[curBranch];
            bool bVidNotButton = currentEvent.dialogues[currentStepIndex - 1].bIsVideo;
            if(bVidNotButton)
            {
                //BranchNarrative(0);
                //Debug.Log("Video Should Start");
                string vidPath = currentEvent.dialogues[currentStepIndex - 1].videoPath;
                StartCoroutine(AssignCurrentRenderTexture.instance.PlayVideoAt(vidPath, VideoCallback, false));
                
            }
            else
            {
                //Debug.Log("Create Answers");
                CreateAnswers();
                //for (int i = 0; i < numberOfPotentialAnswers; i++)
                //{
                //    // Extract button data from current branch config
                //    if (i >= currentBranch.narrativeData.Count)
                //    {

                //        Debug.Log("No More Narrative Data At Branch: " + curBranch);
                //        return;
                //    }
                //    buttonText = currentBranch.narrativeData[i].answerData;
                //    SpawnAnswerButton(i, buttonText);
                //}
            }

            SetCanProceed(false);
            return;
        }
        ScrollUp();

        rpgText.text = currentEvent.dialogues[currentStepIndex].sentence;
        eventTarget = currentEvent.dialogues[currentStepIndex].targetBranchNum;

        // THIS DOESNT WORK QUITE AS I THOUGHT
        // NEED TO MAKE NEW TARGET FOR BRANCH, AND TARGET OF THAT TARGET BRANCH

        // B7T13 == CBN ET

 


        // Okay... this is to make sure, you return to the main branch
        // Assuming a tangent was undergone
        int curBranchID = currentEvent.dialogues[currentStepIndex].currentBranchNum;
        int difference = eventTarget - targetBranchNumber;
        //for (int i = curBranchID; i < testTarget; i++)
        //{
        //    currentAct.IncrementBranchCount();
        //}
        Debug.Log("Diff: " + difference);
        Debug.Log("ET: " + eventTarget);
        Debug.Log("TBN: " + targetBranchNumber);
        Debug.Log("CBN: " + currentEvent.dialogues[currentStepIndex].currentBranchNum);

        currentAct.SetBranchCount(curBranchID);
        //if (difference > 0)
        //{
        //    targetBranchNumber += difference;
        //    for (int i = 0; i < difference; i++)
        //    {
        //        //Debug.Log("Branch Count" + currentAct.GetBranchCount());
        //        currentAct.IncrementBranchCount();
        //    }
        //    //Debug.Log("END Branch Count" + currentAct.GetBranchCount());

        //}

        //int prevBranchNum = eventTarget - targetBranchNumber;
        //int loops = curBranchID - prevBranchNum;
        ////targetBranchNumber += loops;
        //for (int i = 0; i < loops; i++)
        //{
        //    currentAct.IncrementBranchCount();
        //}
        //SetCanProceed(false);
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
