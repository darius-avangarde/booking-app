using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestObject : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    Text text;
    [SerializeField]
    private Image buttonImage;

    public void Initialize(UnityAction clickAction, string message, Color setColor)
    {
        button.onClick.AddListener(clickAction);
        text.text = message;
        buttonImage.color = setColor;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
