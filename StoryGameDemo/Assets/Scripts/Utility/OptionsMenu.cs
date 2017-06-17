using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas[] canvases = new Canvas[2];

    public void EnableFirstCanvas(bool yes)
    {
        if(canvases.Length < 2)
        {
            Debug.Log("Error Options Missing Canvas References");
            return;
        }
        canvases[0].gameObject.SetActive(yes);
        canvases[1].gameObject.SetActive(!yes);
    }

}
