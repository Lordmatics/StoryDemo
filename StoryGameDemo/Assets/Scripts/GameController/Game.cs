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

    private void Update()
    {
        if (previousMessages.Count <= 0) return;
        if(Input.GetKey(KeyCode.DownArrow))
        {
            // Move Scroll down
            //parentForScroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top)
            Vector2 leftBot = parentForScroll.offsetMin;
            Vector2 rightTop = parentForScroll.offsetMax;
            leftBot.y -= 400.0f * Time.deltaTime;
            rightTop.y -= 400.0f * Time.deltaTime;
            Debug.Log(leftBot.y + ": LeftbotY" + " And " + rightTop.y + " : RightTopY");
            
            if(previousMessages.Count < 3)
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
        if(Input.GetKey(KeyCode.UpArrow))
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
        if(currentStepIndex >= currentEvent.dialogues.Count)
        {
            if(OnProceedChanged != null)
            {
                bCanProceed = false;
                OnProceedChanged(bCanProceed);
            }
        }
    }
    #endregion

    void SpawnAnswerButton(int index)
    {
        Debug.Log("SpawnButton");
        GameObject button = (GameObject)Instantiate(Resources.Load("Prefabs/AnswerButton"));
        button.transform.SetParent(parentForAnswers);
        ButtonAnswer script = button.GetComponent<ButtonAnswer>() ? button.GetComponent<ButtonAnswer>() : button.AddComponent<ButtonAnswer>();
        script.SetOptionIndex(index);
        buttonList.Add(button);
    }

    public void DestroyButtons()
    {
        foreach(GameObject obj in buttonList)
        {
            Destroy(obj);
        }
        buttonList.Clear();
    }

    public void BranchNarrative(int index)
    {
        // Reset to start of new event
        currentStepIndex = 0;
        // Need a way to track the branching
        currentEvent = TextFactory.TextAssembly.RunTextFactoryForFile(FileContainer.ActTwo_01);
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
            for (int i = 0; i < numberOfPotentialAnswers; i++)
            {
                SpawnAnswerButton(i);
            }
            bCanProceed = false;
            Debug.Log("Index Out of Bounds");
            return;
        }
        ScrollUp();
        switch(currentEvent.dialogues[currentStepIndex].branchNumber)
        {
            // Reset step index.
            // Load Branching Narrative
            // Assign new Text

            // Continue current branch
            case 0:
                rpgText.text = currentEvent.dialogues[currentStepIndex].lyricString;
                Debug.Log("Case 0 Activated");
                break;
            // Load new branch
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
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
}
