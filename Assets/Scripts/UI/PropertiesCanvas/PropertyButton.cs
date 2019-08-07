using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;

public class PropertyButton : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Image propertyIcon = null;
    [SerializeField]
    private Button propertyButtonItem = null;
    [SerializeField]
    private Sprite[] propertyIcons = null;

    [Header("Theme objects"), SerializeField]
    private Graphic propertyNameText = null;
    [SerializeField]
    private Graphic propertyImage = null;
    [SerializeField]
    private Graphic propertyBackground = null;
    [SerializeField]
    private Shadow shadowComponent = null;

    private IProperty currentProperty;

    private void Start()
    {
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => SetText());
    }

    private void OnDisable()
    {
        propertyButtonItem.onClick.RemoveAllListeners();
    }

    public void InitializeTheme(ThemeManager themeManager)
    {
        themeManager.AddItems(propertyNameText, propertyImage, propertyBackground);
        themeManager.AddShadow(shadowComponent);
    }

    /// <summary>
    /// set the current property
    /// set property name
    /// set button callback for property with or without rooms
    /// </summary>
    /// <param name="property">selected property</param>
    /// <param name="PropertyRoomCallback">click callback for properties without rooms</param>
    /// <param name="PropertyCallback">callback for properties with rooms</param>
    public void Initialize(IProperty property, Action<IProperty> PropertyCallback)
    {
        currentProperty = property;
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : $"{LocalizedText.Instance.PropertyItem}{Environment.NewLine}{currentProperty.Name}";
        for (int i = 0; i < propertyIcons.Length; i++)
        {
            if (propertyIcons[i].name == property.PropertyType.ToString())
            {
                propertyIcon.sprite = propertyIcons[i];
            }
        }
        propertyButtonItem.onClick.AddListener(() => PropertyCallback(property));
    }

    private void SetText()
    {
        propertyName.text = $"{LocalizedText.Instance.PropertyItem}{Environment.NewLine}{currentProperty.Name}";
    }
}