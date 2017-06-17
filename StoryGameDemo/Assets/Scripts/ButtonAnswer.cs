using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Scripts/Game/AnswerButton")]
public class ButtonAnswer : MonoBehaviour , IPointerDownHandler
{

    [SerializeField]
    private int optionIndex = 0;

    [SerializeField]
    private Game script;

    private void Start()
    {
        script = FindObjectOfType<Game>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Branch narrative based off of the option index
        script.BranchNarrative(optionIndex);
        // Remove all buttons, including this one
        script.DestroyButtons();
        //Debug.Log("PointerDown");
    }

    public void SetOptionIndex(int index)
    {
        optionIndex = index;
    }
}
