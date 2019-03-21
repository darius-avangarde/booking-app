using System;
using UnityEngine;

public class FilterDialog : MonoBehaviour, IClosable
{
    [SerializeField]
    private EasyTween easyTween = null;
    [SerializeField]
    private PropertyDropdown propertyDropdown = null;
    [SerializeField]
    private IntegerSelector roomCapacitySelector = null;
    [SerializeField]
    private IntegerSelector singleBedCountSelector = null;
    [SerializeField]
    private IntegerSelector doubleBedCountSelector = null;

    private RoomFilter filter = null;
    private Action<RoomFilter> doneCallback;

    public void Show(RoomFilter filter, Action<RoomFilter> doneCallback)
    {
        easyTween.OpenCloseObjectAnimation();
        propertyDropdown.Initialize();
        this.filter = filter;
        this.doneCallback = doneCallback;

        propertyDropdown.SelectedProperty = PropertyDataManager.GetProperty(filter.PropertyID);
        roomCapacitySelector.Value = filter.RoomCapacity;
        singleBedCountSelector.Value = filter.SingleBeds;
        doubleBedCountSelector.Value = filter.DoubleBeds;

        InputManager.CurrentlyOpenClosable = this;
    }

    public void Close()
    {
        CloseDialog();
    }

    public void ResetFilter()
    {
        filter.Reset();
        propertyDropdown.SelectedProperty = PropertyDataManager.GetProperty(filter.PropertyID);
        roomCapacitySelector.Value = filter.RoomCapacity;
        singleBedCountSelector.Value = filter.SingleBeds;
        doubleBedCountSelector.Value = filter.DoubleBeds;
    }

    public void CloseDialog()
    {
        if (propertyDropdown.SelectedProperty != null)
        {
            filter.PropertyID = propertyDropdown.SelectedProperty.ID;
        }
        filter.RoomCapacity = roomCapacitySelector.Value;
        filter.SingleBeds = singleBedCountSelector.Value;
        filter.DoubleBeds = doubleBedCountSelector.Value;

        doneCallback?.Invoke(filter);
        filter = null;
        doneCallback = null;
        easyTween.OpenCloseObjectAnimation();
    }
}
