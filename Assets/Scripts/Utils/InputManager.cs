using UINavigation;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{

    public static IClosable CurrentlyOpenClosable { get; set; }

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private GameObject QuitOverlayCanvas = null;
    [SerializeField]
    private Image quitObjectImage = null;
    [SerializeField]
    private Text quitObjectText = null;

    private bool clickedBefore = false;
    private Color imageDefaultColor;
    private Color textDefaultColor;
    private Color currentImageCollor;
    private Color currentTextCollor;

    private void Start()
    {
        imageDefaultColor = quitObjectImage.color;
        textDefaultColor = quitObjectText.color;
        currentImageCollor.a = 0;
        currentTextCollor.a = 0;
        quitObjectImage.color = currentImageCollor;
        quitObjectText.color = currentTextCollor;
        QuitOverlayCanvas.SetActive(false);
    }

    private void Update()
    {
        // handle Android back key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentlyOpenClosable != null)
            {
                CurrentlyOpenClosable.Close();
            }
            else
            {
                bool shouldMinimizeApp = navigator.CurrentScreen == navigator.initialScreen;
                if (shouldMinimizeApp && !clickedBefore)
                {
                    //Set to false so that this input is not checked again. It will be checked in the coroutine function instead
                    clickedBefore = true;

                    //Activate Quit Object
                    QuitOverlayCanvas.SetActive(true);
                    quitObjectImage.color = imageDefaultColor;
                    quitObjectText.color = textDefaultColor;

                    //Start quit timer
                    StartCoroutine(QuitingTimer());

                }
                else
                {
                    navigator.GoBack();
                }
            }
        }
    }

    /// <summary>
    /// public function to show a message on the screen
    /// the message will be shown for 1 second and fade
    /// </summary>
    /// <param name="currentText">message text to show</param>
    public void Message(string currentText)
    {
        QuitOverlayCanvas.SetActive(true);
        quitObjectImage.color = imageDefaultColor;
        quitObjectText.color = textDefaultColor;

        StartCoroutine(ShowMessage(currentText));
    }

    /// <summary>
    /// coroutine for the current message
    /// after 1 second the fade coroutine will be called
    /// </summary>
    /// <param name="currentText">message text to show</param>
    /// <returns>wait for 1 second</returns>
    private IEnumerator ShowMessage(string currentText)
    {
        quitObjectText.text = currentText;
        //Wait for a frame so that Input.GetKeyDown is no longer true
        yield return null;

        //1 second timer
        const float timerTime = 1f;
        float counter = 0;

        while (counter < timerTime)
        {
            //Increment counter while it is < timer time(3)
            counter += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(MessageFader());
        clickedBefore = false;
    }

    /// <summary>
    /// coroutine for quit message timer
    /// after 1 second, if the user does not pres escape key, the fade coroutine will start
    /// </summary>
    /// <returns>wait for second press to quit</returns>
    private IEnumerator QuitingTimer()
    {
        quitObjectText.text = "Apăsați încă o dată pentru a închide!";
        //Wait for a frame so that Input.GetKeyDown is no longer true
        yield return null;

        //1 second timer
        const float timerTime = 1f;
        float counter = 0;

        while (counter < timerTime)
        {
            //Increment counter while it is < timer time(3)
            counter += Time.deltaTime;

            //Check if Input is pressed again while timer is running then quit/exit if is
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Debug.Log("Back Button pressed for the second time. EXITING.....");
                Quit();
            }

            //Wait for a frame so that Unity does not freeze
            yield return null;
        }

        //Timer has finished and NO QUIT(NO second press) so deactivate
        StartCoroutine(MessageFader());
        //Reset clickedBefore so that Input can be checked again in the Update function
        clickedBefore = false;
    }

    /// <summary>
    /// coroutine to fade the current message from the screen
    /// </summary>
    /// <returns>wait for end of frame</returns>
    private IEnumerator MessageFader()
    {
        currentImageCollor = quitObjectImage.color;
        currentTextCollor = quitObjectText.color;
        float alpha = currentImageCollor.a;
        while (currentTextCollor.a >= 0)
        {
            alpha -= Time.deltaTime * 2;
            currentImageCollor.a = alpha;
            currentTextCollor.a = alpha;
            quitObjectImage.color = currentImageCollor;
            quitObjectText.color = currentTextCollor;
            yield return null;
        }
        currentImageCollor.a = 0;
        currentTextCollor.a = 0;
        quitObjectImage.color = currentImageCollor;
        quitObjectText.color = currentTextCollor;
        QuitOverlayCanvas.SetActive(false);
    }

    /// <summary>
    /// close the app function
    /// </summary>
    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    //Application.Quit();
    System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    //inactive original code
    #region OriginalCode
    // // modal dialogs need to set themselves as CurrentlyOpenClosable when they're shown and unset themselves when they're hidden
    // // it's a good idea to do this right after easyTween.OpenCloseObjectAnimation
    // public static IClosable CurrentlyOpenClosable { get; set; }

    // [SerializeField]
    // private SideMenu sideMenu = null;

    // void Update()
    // {
    //     // handle Android back key
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         if (CurrentlyOpenClosable != null)
    //         {
    //             CurrentlyOpenClosable.Close();
    //         }
    //         else
    //         {
    //             Navigator navigator = sideMenu.CurrentlyActiveNavigator;
    //             bool shouldMinimizeApp = navigator.CurrentScreen == navigator.initialScreen;
    //             if (shouldMinimizeApp)
    //             {
    //                 Application.Quit();
    //             }
    //             else
    //             {
    //                 sideMenu.CurrentlyActiveNavigator.GoBack();
    //             }
    //         }
    //     }
    // }
    #endregion
}
