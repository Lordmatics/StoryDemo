using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Utility/ResetAnchors")]
public class ResetAnchors : MonoBehaviour
{

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.ResetTransform();
        transform.parent.gameObject.SetActive(false);
    }
    void Start ()
    {
		
	}
	
}
