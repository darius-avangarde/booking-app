using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    public static IProperty currentProperty;
    [SerializeField]
    private Text propertyScreenTitle;
    [SerializeField]
    private Text propertyNamePlaceholder;
    [SerializeField]
    private InputField propertyNameInputField;
    [SerializeField]
    private GameObject roomPrefabButton;
    [SerializeField]
    private Transform roomsContentScrollView;

    // Start is called before the first frame update
    void Start()
    {
        //propertyScreenTitle.text = currentProperty.Name == "" ? propertyScreenTitle.text : currentProperty.Name;
        propertyNameInputField.onValueChanged.AddListener(delegate { OnValueChanged();});
        InstantiateRooms();
    }

    public void AddRoomItem()
    {
        IRoom room = currentProperty.AddRoom();
    }

    public void DeleteProperty()
    {
        PropertyDataManager.DeleteProperty(currentProperty.ID);
    }

    private void OnValueChanged()
    {
        propertyScreenTitle.text = propertyNameInputField.text;
    }

    private void InstantiateRooms()
    {
        foreach (var property in PropertyDataManager.GetProperties())
        {
            foreach (var room in property.Rooms)
            {
                GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
                roomButton.GetComponent<RoomFields>().Initialize(room);
            }
        }
    }
}
