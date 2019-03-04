using UINavigation;
using UnityEngine;

public class SideMenu : MonoBehaviour
{
    [SerializeField]
    private EasyTween sideMenu = null;

    [Space]
    [SerializeField]
    private GameObject reservationsCanvas = null;
    [SerializeField]
    private Navigator reservationsNavigator = null;
    [SerializeField]
    private SideMenuButton reservationsButton = null;

    [Space]
    [SerializeField]
    private GameObject propertiesCanvas = null;
    [SerializeField]
    private Navigator propertiesNavigator = null;
    [SerializeField]
    private SideMenuButton propertiesButton = null;

    [Space]
    [SerializeField]
    private GameObject statisticsCanvas = null;
    [SerializeField]
    private Navigator statisticsNavigator = null;
    [SerializeField]
    private SideMenuButton statisticsButton = null;

    [Space]
    [SerializeField]
    private GameObject settingsCanvas = null;
    [SerializeField]
    private Navigator settingsNavigator = null;
    [SerializeField]
    private SideMenuButton settingsButton = null;

    [Space]
    [SerializeField]
    private GameObject infoCanvas = null;
    [SerializeField]
    private Navigator infoNavigator = null;
    [SerializeField]
    private SideMenuButton infoButton = null;

    private GameObject currentlyActiveCanvas = null;
    private SideMenuButton currentlyActiveButton = null;

    private void Start()
    {
        currentlyActiveCanvas = reservationsCanvas;
        reservationsButton.Active = true;
        currentlyActiveButton = reservationsButton;

        // currentlyActiveCanvas = propertiesCanvas;
        // propertiesButton.Active = true;
        // currentlyActiveButton = propertiesButton;
    }

    public void ShowReservations()
    {
        reservationsNavigator.Reset();

        currentlyActiveCanvas.SetActive(false);
        reservationsCanvas.SetActive(true);
        currentlyActiveCanvas = reservationsCanvas;

        currentlyActiveButton.Active = false;
        reservationsButton.Active = true;
        currentlyActiveButton = reservationsButton;

        sideMenu.OpenCloseObjectAnimation();
    }

    public void ShowProperties()
    {
        propertiesNavigator.Reset();

        currentlyActiveCanvas.SetActive(false);
        propertiesCanvas.SetActive(true);
        currentlyActiveCanvas = propertiesCanvas;

        currentlyActiveButton.Active = false;
        propertiesButton.Active = true;
        currentlyActiveButton = propertiesButton;

        sideMenu.OpenCloseObjectAnimation();
    }

    public void ShowStatistics()
    {
        statisticsNavigator.Reset();

        currentlyActiveCanvas.SetActive(false);
        statisticsCanvas.SetActive(true);
        currentlyActiveCanvas = statisticsCanvas;

        currentlyActiveButton.Active = false;
        statisticsButton.Active = true;
        currentlyActiveButton = statisticsButton;

        sideMenu.OpenCloseObjectAnimation();
    }

    public void ShowSettings()
    {
        settingsNavigator.Reset();

        currentlyActiveCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
        currentlyActiveCanvas = settingsCanvas;

        currentlyActiveButton.Active = false;
        settingsButton.Active = true;
        currentlyActiveButton = settingsButton;

        sideMenu.OpenCloseObjectAnimation();
    }

    public void ShowInfo()
    {
        infoNavigator.Reset();

        currentlyActiveCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        currentlyActiveCanvas = infoCanvas;

        currentlyActiveButton.Active = false;
        infoButton.Active = true;
        currentlyActiveButton = infoButton;

        sideMenu.OpenCloseObjectAnimation();
    }

    public void Show()
    {
        sideMenu.OpenCloseObjectAnimation();
    }
}
