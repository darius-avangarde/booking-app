using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour
{
    private InputField input;
    private string lastText;

    private void Start()
    {
        input = GetComponent<InputField>();
#if !UNITY_EDITOR
        input.onValueChanged.AddListener(delegate { SetText(); });
        input.onEndEdit.AddListener(delegate { SaveText(); });
#endif
    }

    private void SaveText()
    {
        if (input.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            input.text = lastText;
        }
    }

    private void SetText()
    {
        if (input.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
            return;

        lastText = input.touchScreenKeyboard.text;
        input.text = lastText;
    }
}

