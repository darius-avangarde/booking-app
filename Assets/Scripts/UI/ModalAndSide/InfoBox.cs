using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour, IClosable
{
    [SerializeField]
    private Text messageText = null;
    [SerializeField]
    private ModalFadeObject modalFadeObject = null;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string text)
    {
        messageText.text = text;
        modalFadeObject.FadeIn();
        InputManager.CurrentlyOpenClosable = this;
    }

    public void Close()
    {
        modalFadeObject.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
    }
}
