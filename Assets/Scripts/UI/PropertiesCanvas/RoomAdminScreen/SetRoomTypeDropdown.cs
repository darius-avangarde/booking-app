using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRoomTypeDropdown : MonoBehaviour
{
    [SerializeField]
    private Dropdown roomTypeDropdown = null;
    [SerializeField]
    private Sprite[] roomTypeIcons;

    public PropertyDataManager.RoomType CurrentRoomType
    {
        get => (PropertyDataManager.RoomType)roomTypeDropdown.value;
        set
        {
            roomTypeDropdown.value = (int)value;
        }
    }

    private Dictionary<PropertyDataManager.RoomType, string> roomTypeName = new Dictionary<PropertyDataManager.RoomType, string>();
    private Dictionary<string, Sprite> roomIcons = new Dictionary<string, Sprite>();
    private string[] roomTypeText = { "Cameră", "Apartament", "Vila", "Cabană", "Pat" };

    private void Awake()
    {
        SetDictionaries();
        SetDropdownOptions();
    }

    /// <summary>
    /// set every icon to a dictionary
    /// for loop goes from 0 to last element from RoomType enum wich is bed
    /// RoomType enum is found in PropertyDataManager
    /// </summary>
    private void SetDictionaries()
    {
        foreach (var icon in roomTypeIcons)
        {
            roomIcons.Add(icon.name.ToLower(), icon);
        }
        for (int i = 0; i <= (int)PropertyDataManager.RoomType.apartment; i++)
        {
            roomTypeName[(PropertyDataManager.RoomType)i] = roomTypeText[i];
        }
    }

    /// <summary>
    /// set dropdown options with text and icons
    /// for every item in the enum, find the coresponding icon and text using dictionaries
    /// </summary>
    private void SetDropdownOptions()
    {
        roomTypeDropdown.options.Clear();
        for (int i = 0; i <= (int)PropertyDataManager.RoomType.apartment; i++)
        {
            roomTypeName[(PropertyDataManager.RoomType)i] = roomTypeText[i];
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = roomTypeText[i];
            foreach (var icon in roomIcons)
            {
                if (icon.Key == ((PropertyDataManager.RoomType)i).ToString())
                {
                    newOption.image = icon.Value;
                }
            }
            roomTypeDropdown.options.Add(newOption);
        }
    }
}
