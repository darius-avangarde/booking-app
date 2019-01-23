using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class DayScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform filteredPropertiesContent = null;
    [SerializeField]
    private GameObject dayScreenItemPrefab = null;
    [SerializeField]
    private Transform roomScreen = null;
    [SerializeField]
    private Text dayScreenTitle = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject dayScreenItem = Instantiate(dayScreenItemPrefab, filteredPropertiesContent);
            dayScreenItem.GetComponent<DayScreenPropertyItem>().Initialize(property, () => OpenRoomReservationScreen(property));
        }
    }

    private void OpenRoomReservationScreen(IProperty property)
    {
        roomScreen.GetComponent<RoomScreen>().UpdatePropertyFields(property);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
