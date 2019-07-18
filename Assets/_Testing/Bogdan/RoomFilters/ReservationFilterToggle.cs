using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReservationFilterToggle : MonoBehaviour
{
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }
}
