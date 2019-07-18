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

    public void SetCurrentBeds(int singleBeds, int doubleBeds)
    {
        SingleBedsNr = singleBeds;
        DoubleBedsNr = doubleBeds;
        roomSingleBedQuantityInputField.text = SingleBedsNr.ToString();
        roomDoubleBedQuantityInputField.text = DoubleBedsNr.ToString();
    }

    /// <summary>
    /// returns single beds as X component
    /// returns double beds as Y component
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCurrentBeds()
    {
        Vector2 bedsInfo = new Vector2(SingleBedsNr, DoubleBedsNr);
        return bedsInfo;
    }

    public void OnSingleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        SingleBedsNr = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void OnDoubleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        DoubleBedsNr = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void IncrementSingleBedQuantity()
    {
        roomSingleBedQuantityInputField.text = (++SingleBedsNr).ToString();
    }

    public void DecrementSingleBedQuantity()
    {
        if (roomSingleBedQuantityInputField.text != "0")
        {
            roomSingleBedQuantityInputField.text = (--SingleBedsNr).ToString();
        }
    }

    public void IncrementDoubleBedQuantity()
    {
        roomDoubleBedQuantityInputField.text = (++DoubleBedsNr).ToString();
    }

    public void DecrementDoubleBedQuantity()
    {
        if (roomDoubleBedQuantityInputField.text != "0")
        {
            roomDoubleBedQuantityInputField.text = (--DoubleBedsNr).ToString();
        }
    }
}
