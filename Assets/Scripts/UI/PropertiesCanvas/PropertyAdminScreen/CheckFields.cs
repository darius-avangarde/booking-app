using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckFields : MonoBehaviour
{
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreenComponent;
    [SerializeField]
    private InputField propertyNameField;
    [SerializeField]
    private InputField multipleRoomsField = null;
    [SerializeField]
    private Text errorMessage = null;

    private void Start()
    {
        propertyAdminScreenComponent.CheckSave += CheckNameAndRooms;
    }

    private void OnEnable()
    {
        ResetError();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="canSave">bool to check if no error was returned</param>
    /// <param name="error">error message</param>
    private void CheckNameAndRooms()
    {
        if (string.IsNullOrEmpty(propertyNameField.text))
        {
            errorMessage.text = Constants.ERR_PROPERTY_NAME;
            propertyAdminScreenComponent.CanSave = false;
            return;
        }
        else if (string.IsNullOrEmpty(multipleRoomsField.text))
        {
            errorMessage.text = Constants.ERR_ROOM_NUMBER;
            propertyAdminScreenComponent.CanSave = false;
            return;
        }
        else if (int.Parse(multipleRoomsField.text) <= 0)
        {
            errorMessage.text = Constants.ERR_ROOM_NULL_NUMBER;
            propertyAdminScreenComponent.CanSave = false;
            return;
        }
        else
        {
            errorMessage.text = string.Empty;
            propertyAdminScreenComponent.CanSave = true;
        }
    }

    /// <summary>
    /// reset error message and bool to default
    /// </summary>
    public void ResetError()
    {
        propertyAdminScreenComponent.CanSave = true;
        errorMessage.text = string.Empty;
    }
}
