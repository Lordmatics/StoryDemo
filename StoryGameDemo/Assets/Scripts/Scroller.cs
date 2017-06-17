using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Game/Scroller")]
public class Scroller : MonoBehaviour
{

    private RectTransform rect;

    [SerializeField]
    private float lerpSpeed = 5.0f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Invoke("LateEnable", 0.5f);
    }

    void LateEnable()
    {
        Game.OnScrollFinished += SetCanProceed;
    }

    private void OnDisable()
    {
        Game.OnScrollFinished -= SetCanProceed;
    }

    public IEnumerator InitiateScroll()
    {
        float targetMinY = rect.anchorMin.y + 0.35f;
        float targetMaxY = rect.anchorMax.y + 0.35f;
        while (rect.anchorMin.y < targetMinY && rect.anchorMax.y < targetMaxY)
        {
            float newMinY = rect.anchorMin.y;
            newMinY = Mathf.Lerp(rect.anchorMin.y, targetMinY + 0.05f, lerpSpeed * Time.deltaTime);
            //newMinY = Mathf.Clamp(rect.anchorMin.y, 0.0f, targetMinY);
            rect.anchorMin = new Vector2(0.0f, newMinY);

            //rect.ResetTransform();

            float newMaxY = rect.anchorMax.y;
            newMaxY = Mathf.Lerp(rect.anchorMax.y, targetMaxY + 0.05f, lerpSpeed * Time.deltaTime);
            //newMaxY = Mathf.Clamp(rect.anchorMax.y, 0.0f, targetMaxY);
            rect.anchorMax = new Vector2(1.0f, newMaxY);

            rect.ResetTransform();

            yield return null;
        }
        if(Game.OnScrollFinished != null)
            Game.OnScrollFinished();
        //Game.bCanProceed = true;
    }

    void SetCanProceed()
    {
        Game.bCanProceed = true;
    }
}
