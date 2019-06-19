using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleDialogObj : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Text toggleLabel;


    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }

    public void SetObject(string optionText, ToggleGroup group, UnityAction<bool> selectCallback)
    {
        toggleLabel.text = optionText;
        toggle.group = group;
        toggle.onValueChanged.AddListener(selectCallback);
    }
}
