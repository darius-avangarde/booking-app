using System.Collections;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    public PropertiesScreen propertiesScreen { get; set; }
    public DisponibilityScreen disponibilityScreen { get; set; }

    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Text propertyScreenTitle = null;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private Toggle HasRoomsToggle;
    [SerializeField]
    private Toggle NoRoomsToggle;
    [SerializeField]
    private GameObject RoomsToggleField;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button calcelButton;
    private IProperty currentProperty;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        if(currentProperty.Name != null)
        {
            RoomsToggleField.SetActive(false);
        }
        else
        {
            RoomsToggleField.SetActive(true);
        }
    }

    public void SetPropertyFieldsText()
    {
        propertyNameInputField.text = currentProperty.Name ?? "";
        propertyScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
        if (currentProperty.HasRooms)
        {
            HasRoomsToggle.isOn = true;
            NoRoomsToggle.isOn = false;
        }
        else
        {
            NoRoomsToggle.isOn = true;
            HasRoomsToggle.isOn = false;
        }
    }

    public void SaveProperty()
    {
        NameChanged(propertyNameInputField.text);
        if (HasRoomsToggle.isOn)
        {
            currentProperty.HasRooms = true;
        }
        else
        {
            currentProperty.HasRooms = false;
        }
        if (PropertyDataManager.GetProperty(currentProperty.ID) == null)
        {
            PropertyDataManager.SaveProperty(currentProperty);
        }
        OpenPropertiesScreen();
    }

    private void NameChanged(string value)
    {
        propertyScreenTitle.text = value;
        currentProperty.Name = string.IsNullOrEmpty(value) ? Constants.NEW_PROPERTY : value;
    }

    private void OpenPropertiesScreen()
    {
        if (propertiesScreen != null)
        {
            propertiesScreen.Initialize();
            propertiesScreen = null;
            navigator.GoBack();
        }
        if (disponibilityScreen != null)
        {
            disponibilityScreen.Initialize();
            disponibilityScreen = null;
            navigator.GoBack();
        }
    }
}
