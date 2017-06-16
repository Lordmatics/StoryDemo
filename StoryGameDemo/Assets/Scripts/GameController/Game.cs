using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[AddComponentMenu("Scripts/Game/Game")]
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
    private List<RectTransform> previousMessages = new List<RectTransform>();

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
    }

    private void Start()
    {
        currentEvent = TextFactory.TextAssembly.RunTextFactoryForFile(FileContainer.TextName);
        rpgText.text = currentEvent.dialogues[currentStepIndex++].lyricString;  
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

            // Move scroll up
            //Vector2 pos = parentForScroll.sizeDelta;
            //pos.y += scrollSpeed * Time.deltaTime;
            //parentForScroll.sizeDelta = pos;
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

    public void ProceedThroughDialogueEvent(int selectedOption)
    {
        if (!bCanProceed) return;
        if (currentStepIndex >= currentEvent.dialogues.Count) return;

        ScrollUp();
        switch(currentEvent.dialogues[currentStepIndex].selectedOption)
        {
            // Reset step index.
            // Load Branching Narrative
            // Assign new Text

            // Continue current branch
            case 0:
                rpgText.text = currentEvent.dialogues[currentStepIndex].lyricString;
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
        RectTransform rect = message.GetComponent<RectTransform>();

        message.transform.SetParent(parentForScroll);
        rect.anchorMin = new Vector2(0.0f, -0.35f);
        rect.anchorMax = new Vector2(1.0f, 0.0f);
        //rect.ResetTransform();

        previousMessages.Add(rect);
        //StartCoroutine(PerformScroll());
        // Add to scroll container
        // Lerp all contents up

        foreach (RectTransform t in previousMessages)
        {
            Scroller scroll = t.GetComponent<Scroller>();
            if (scroll != null)
            {
                StartCoroutine(scroll.InitiateScroll());
            }
        }
    }

    IEnumerator PerformScroll()
    {
        yield return null;
        foreach(RectTransform t in previousMessages)
        {
            //float targetMinY = t.anchorMin.y + 0.35f;
            //float targetMaxY = t.anchorMax.y + 0.35f;
            //while(t.anchorMin.y < targetMinY && t.anchorMax.y < targetMaxY)
            //{
            //    float newMinY = t.anchorMin.y;
            //    newMinY = Mathf.Lerp(t.anchorMin.y, targetMinY + 0.01f, lerpSpeed * Time.deltaTime);
            //    t.anchorMin = new Vector2(0.0f, newMinY);

            //    //t.ResetTransform();

            //    float newMaxY = t.anchorMax.y;
            //    newMaxY = Mathf.Lerp(t.anchorMax.y, targetMaxY + 0.01f, lerpSpeed * Time.deltaTime);
            //    t.anchorMax = new Vector2(1.0f, newMaxY);

            //    t.ResetTransform();

            //    yield return null;
            //}
        }
        //rect.anchorMin = new Vector2(0.0f, 0.0f);
        //rect.anchorMax = new Vector2(1.0f, 0.35f);
        //rect.ResetTransform();

        //Vector2 newAnchorMin = new Vector2(previousMessages[0].anchorMin.x, previousMessages[0].anchorMin.y + 0.35f);
        //Vector2 newAnchorMax = new Vector2(previousMessages[0].anchorMax.x, previousMessages[0].anchorMax.x + 0.35f);

        //float newMinY = previousMessages[0].anchorMin.y + 0.35f;
        //float newMaxY = previousMessages[0].anchorMax.y + 0.35f;

        //float target = boxToDuplicate.transform.position.y + 200.0f;
        //while(previousMessages[0].transform.position.y != target)
        //{
        //    foreach(RectTransform obj in previousMessages)
        //    {
        //        Vector2 pos = obj.anchoredPosition;
        //        pos.y += 200.0f / 2.0f;
        //        yield return null;
        //    }
        //}
    }
}
