using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBedsNumber : MonoBehaviour
{
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;

    private int SingleBedsNr;
    private int DoubleBedsNr;

    /// <summary>
    /// returns single beds as X component
    /// returns double beds as Y component
    /// </summary>
    public void SetCurrentBeds(Vector2Int bedInfo)
    {
        SingleBedsNr = bedInfo.x;
        DoubleBedsNr = bedInfo.y;
        roomSingleBedQuantityInputField.text = SingleBedsNr.ToString();
        roomDoubleBedQuantityInputField.text = DoubleBedsNr.ToString();
    }

    /// <summary>
    /// returns single beds as X component
    /// returns double beds as Y component
    /// </summary>
    public Vector2Int GetCurrentBeds()
    {
        Vector2Int bedsInfo = new Vector2Int(SingleBedsNr, DoubleBedsNr);
        return bedsInfo;
    }

    /// <summary>
    /// function for single beds input field on value changed
    /// </summary>
    /// <param name="value"></param>
    public void OnSingleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        SingleBedsNr = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }
    
    /// <summary>
    /// function for double beds input field on value changed
    /// </summary>
    /// <param name="value"></param>
    public void OnDoubleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        DoubleBedsNr = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    /// <summary>
    /// function for + button for single beds
    /// </summary>
    public void IncrementSingleBedQuantity()
    {
        roomSingleBedQuantityInputField.text = (++SingleBedsNr).ToString();
    }

    /// <summary>
    /// function for - button for single beds
    /// </summary>
    public void DecrementSingleBedQuantity()
    {
        if (roomSingleBedQuantityInputField.text != "0")
        {
            roomSingleBedQuantityInputField.text = (--SingleBedsNr).ToString();
        }
    }

    /// <summary>
    /// function for + button for double beds
    /// </summary>
    public void IncrementDoubleBedQuantity()
    {
        roomDoubleBedQuantityInputField.text = (++DoubleBedsNr).ToString();
    }

    /// <summary>
    /// function for - button for double beds
    /// </summary>
    public void DecrementDoubleBedQuantity()
    {
        if (roomDoubleBedQuantityInputField.text != "0")
        {
            roomDoubleBedQuantityInputField.text = (--DoubleBedsNr).ToString();
        }
    }
}
