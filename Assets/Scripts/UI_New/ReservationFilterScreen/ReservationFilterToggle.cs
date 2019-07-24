using UnityEngine;
using UnityEngine.UI;

public class ReservationFilterToggle : MonoBehaviour
{

    public bool IsOn => toggleObject.isOn;

    [SerializeField]
    private Toggle toggleObject;
    [SerializeField]
    private GameObject toggleingPanel;
    [SerializeField]
    private RectTransform parentRect;



    private void Start()
    {
        Toggle(false);
        toggleObject.onValueChanged.AddListener(OnToggle);
    }


    private void OnToggle(bool toggleState)
    {
        if(gameObject.activeInHierarchy)
            Toggle(toggleState);
    }

    public void Toggle(bool show)
    {
        toggleingPanel.SetActive(show);
        toggleObject.isOn = show;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }
}
