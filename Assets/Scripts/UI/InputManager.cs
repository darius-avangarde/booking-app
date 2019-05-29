using UINavigation;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static IClosable CurrentlyOpenClosable { get; set; }

    [SerializeField]
    private Navigator navigator = null;

    void Update()
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
                if (shouldMinimizeApp)
                {
                    Application.Quit();
                }
                else
                {
                    navigator.GoBack();
                }
            }
        }
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
