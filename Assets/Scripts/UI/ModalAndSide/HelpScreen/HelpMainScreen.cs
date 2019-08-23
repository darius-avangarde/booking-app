using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UINavigation;

public class HelpMainScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private HelpInfoScreen helpInfoScreen = null;
    [SerializeField]
    private Button backButton = null;

    private NavScreen helpInfoNavScreen;

    private void Start()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        helpInfoNavScreen = helpInfoScreen.GetComponent<NavScreen>();
    }

    /// <summary>
    /// function to open the selected info page
    /// </summary>
    /// <param name="buttonText">name of the info page to open</param>
    public void OpenInfo(string buttonText)
    {
        navigator.GoTo(helpInfoNavScreen);
        helpInfoScreen.Initialize(buttonText);
    }
}
