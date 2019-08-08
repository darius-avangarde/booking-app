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
            roomTypeDropdown.RefreshShownValue();
        }
    }

    private Dictionary<PropertyDataManager.RoomType, string> roomTypeName = new Dictionary<PropertyDataManager.RoomType, string>();
    private Dictionary<string, Sprite> roomIcons = new Dictionary<string, Sprite>();

    private void Start()
    {
        SetDictionaries();
        SetDropdownOptions();
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => SetDropdownOptions());
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => UpdateRoomTypeDropdown());
    }

    /// <summary>
    /// set every icon to a dictionary
    /// for loop goes from 0 to last element from RoomType enum wich is bed
    /// RoomType enum is found in PropertyDataManager
    /// </summary>
    private void SetDictionaries()
    {
        for (int i = 0; i < roomTypeIcons.Length; i++)
        {
            roomIcons.Add(roomTypeIcons[i].name.ToLower(), roomTypeIcons[i]);
        }
        for (int i = 0; i <= (int)PropertyDataManager.RoomType.apartment; i++)
        {
            roomTypeName[(PropertyDataManager.RoomType)i] = LocalizedText.Instance.RoomType[i];
        }
    }

    private void UpdateRoomTypeDropdown()
    {
        for (int i = 0; i <= (int)PropertyDataManager.RoomType.apartment; i++)
        {
            roomTypeName[(PropertyDataManager.RoomType)i] = LocalizedText.Instance.RoomType[i];
        }
        SetDropdownOptions();
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
            roomTypeName[(PropertyDataManager.RoomType)i] = LocalizedText.Instance.RoomType[i];
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = LocalizedText.Instance.RoomType[i];
            foreach (KeyValuePair<string, Sprite> icon in roomIcons)
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
