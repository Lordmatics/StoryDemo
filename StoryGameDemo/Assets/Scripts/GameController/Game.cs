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
        currentEvent = TextFactory.TextAssembly.RunTextFactory<DialogueEvent>(FileContainer.ActTwo_01);
        rpgText.text = currentEvent.dialogues[currentStepIndex++].sentence;
        Debug.Log("Count" + currentEvent.dialogues.Count + " stepindex " + currentStepIndex);
    }

    #region DEBUG
    public Text curActText;
    public Text curBranchNum;
    public Text curBranchIndex;
    public Text curBranchTotal;
    public Text curData;
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
        if (currentStepIndex > currentEvent.dialogues.Count && !bVideo)
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

    Branch GetBranch()
    {
        Act currentAct = Acts.instance.GetCurrentAct();
        int curBranchIndex = currentAct.GetBranchCount();
        Branch curBranch = currentAct.GetBranches()[curBranchIndex];
        return curBranch;
    }

    // Answer Button index
    // Pretty much resets the Dialogue Event and StepIndex
    // And increases the Branch Count to progress through the narrative
    public void BranchNarrative(int index)
    {
        Act currentAct = Acts.instance.GetCurrentAct();
        int curBranchIndex = currentAct.GetBranchCount();
        Branch curBranch = currentAct.GetBranches()[curBranchIndex];
        string filePath = curBranch.narrativeData[index].filePath;
        currentStepIndex = 0;
        // Linear Progression on main branch - This will be auto adjusted if a side branch is taken
        currentAct.IncrementBranchCount();
        currentEvent = TextFactory.TextAssembly.RunTextFactory<DialogueEvent>(filePath);
        SetCanProceed(true);
        // Makes video autoplay
        if (currentEvent.dialogues[currentStepIndex].bIsVideo)
        {
            ProceedThroughDialogueEvent();
        }
        // Load next dialogue
        ProceedThroughDialogueEvent();
    }

    // Bind functions to this to pass through
    // As an event for on video finished
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
        // Prevent this from being repeated maybe? - BUG SPAMMING CLICK SPAWNS LOTS OF BUTTONS
        List<NarrativeData> currBranchData = GetBranch().narrativeData;
        for (int i = 0; i < currentEvent.dialogues[currentStepIndex - 1].possibleAnswersNum; i++)
        {
            // Extract button data from current branch config
            if (i >= currBranchData.Count)
            {
                return;
            }
            // Text to go on button and option index
            SpawnAnswerButton(i, currBranchData[i].answerData);
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
        Debug.Log("-----------------CurrentStepIndex:" + currentStepIndex + " Count: " + currentEvent.dialogues.Count);
        if (currentStepIndex >= currentEvent.dialogues.Count)
        {
            // Need a constraint here, if there are no more narrative entries for this act
            bool bVidNotButton = currentEvent.dialogues[currentStepIndex - 1].bIsVideo;
            if (bVidNotButton)
            {
                string vidPath = currentEvent.dialogues[currentStepIndex - 1].videoPath;
                StartCoroutine(AssignCurrentRenderTexture.instance.PlayVideoAt(vidPath, VideoCallback, false));            
            }
            else
            {
                CreateAnswers();
            }

            SetCanProceed(false);
            return;
        }
        UpdateNarrativeActAndBranch();
    }

    void UpdateNarrativeActAndBranch()
    {
        // This fixes the "Continue" bug. And - Fixes the duplicate answers being spawned
        // Logic being, if a video is expected to play next, disregard the text box logic
        bool bVideo = currentEvent.dialogues[currentStepIndex].bIsVideo;
        if (!bVideo)
        {
            ScrollUp();
            rpgText.text = currentEvent.dialogues[currentStepIndex].sentence;
        }
        int curBranchID = currentEvent.dialogues[currentStepIndex].currentBranchNum;
        Act currentAct = Acts.instance.GetCurrentAct();
        currentAct.SetBranchCount(curBranchID);
        IncrementStepIndex();
    }

    void ScrollUp()
    {
        SetCanProceed(false);
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

    public void SetCanProceed(bool canProceed)
    {
        bCanProceed = canProceed;
        if (OnProceedChanged != null)
            OnProceedChanged(bCanProceed);
    }
}
