using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Scripts/Game/TextBoxPressed")]
public class TextBoxPressed : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Game script;

    private void Start()
    {
        script = FindObjectOfType<Game>();    
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        script.ProceedThroughDialogueEvent();
    }
}
