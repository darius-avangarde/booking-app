using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour, IClosable
{
    [SerializeField]
    private Text messageText = null;
    [SerializeField]
    private EasyTween easyTween = null;

    public void Show(string text)
    {
        messageText.text = text;
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = this;
    }

    public void Close()
    {
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = null;
    }
}
