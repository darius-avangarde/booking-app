using UINavigation;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static IClosable CurrentlyOpenClosable { get; set; }

    [SerializeField]
    private SideMenu sideMenu = null;

    void Update()
    {
        // handle Android back key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentlyOpenClosable != null)
            {
                CurrentlyOpenClosable.Close();
                CurrentlyOpenClosable = null;
            }
            else
            {
                Navigator navigator = sideMenu.CurrentlyActiveNavigator;
                bool shouldMinimizeApp = navigator.CurrentScreen == navigator.initialScreen;
                if (shouldMinimizeApp)
                {
                    Minimize();
                }
                else
                {
                    sideMenu.CurrentlyActiveNavigator.GoBack();
                }
            }
        }
    }

    private void Minimize()
    {
        Debug.Log("[DEBUG] Minimize ");
    }
}
