using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Scripts/Game/CanProceedFeedback")]
public class CanProceedFeedback : MonoBehaviour
{

    [SerializeField]
    private Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
        icon.enabled = Game.bCanProceed;
    }

    private void OnEnable()
    {
        Invoke("LateEnable", 0.25f);
    }

    void LateEnable()
    {
        Game.OnProceedChanged += SetIcon;
    }

    private void OnDisable()
    {
        Game.OnProceedChanged -= SetIcon;
    }

    #region GameEvents
    void SetIcon(bool enabled)
    {
        icon.enabled = enabled;
        //Debug.Log("Icon Enabled : " + enabled);
    }
    #endregion
}
