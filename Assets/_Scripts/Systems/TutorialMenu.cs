using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public GameObject TutorialUI;

    public void OpenSettings()
    {
        TutorialUI.SetActive(true);
    }

    public void CloseSettings()
    {
        TutorialUI.SetActive(false);
    }
}
