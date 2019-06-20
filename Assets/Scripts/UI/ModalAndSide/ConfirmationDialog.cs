using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour, IClosable
{
    [SerializeField]
    private ModalFadeObject modalFade = null;
    [SerializeField]
    private Text messageText = null;
    [SerializeField]
    private Text cancelButtonText = null;
    [SerializeField]
    private Text confirmButtonText = null;
    [SerializeField]
    private Text confirmSecondButtonText = null;
    [SerializeField]
    private GameObject confirmSecondButton = null;
    [SerializeField]
    private HorizontalLayoutGroup ButtonsLayoutGroup = null;

    public string defaultMessage = "Sunteți sigur?";
    public string defaultConfirmButtonText = "Confirmați";
    public string defaultCancelButtonText = "Anulați";

    private Action ConfirmCallback;
    private Action ConfirmCallbackSecond;
    private Action CancelCallback;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Reset()
    {
        ConfirmCallback = null;
        ConfirmCallbackSecond = null;
        CancelCallback = null;
    }

    public void Show(ConfirmationDialogOptions options)
    {
        if(options.AditionalCallback)
        {
            confirmSecondButtonText.text = options.ConfirmTextSecond;
            ConfirmCallbackSecond = options.ConfirmCallbackSecond;
            confirmSecondButton.SetActive(true);
            ButtonsLayoutGroup.padding.left = -40;
            ButtonsLayoutGroup.padding.right = -40;
        }
        else
        {
            confirmSecondButton.SetActive(false);
            ButtonsLayoutGroup.padding.left = 100;
            ButtonsLayoutGroup.padding.right = 100;
        }

        messageText.text = options.Message ?? defaultMessage;
        confirmButtonText.text = options.ConfirmText ?? defaultConfirmButtonText;
        cancelButtonText.text = options.CancelText ?? defaultCancelButtonText;
        ConfirmCallback = options.ConfirmCallback;
        CancelCallback = options.CancelCallback;
        modalFade.FadeIn();
        InputManager.CurrentlyOpenClosable = this;
    }

    public void Confirm()
    {
        ConfirmCallback?.Invoke();
        modalFade.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
        Reset();
    }

    public void ConfirmSecond()
    {
        ConfirmCallbackSecond?.Invoke();
        modalFade.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
        Reset();
    }

    public void Cancel()
    {
        CancelCallback?.Invoke();
        modalFade.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
        Reset();
    }

    public void Close()
    {
        Cancel();
    }
}

public class ConfirmationDialogOptions
{
    public bool AditionalCallback { get; set;} = false;
    public string Message { get; set; }
    public string ConfirmText { get; set; }
    public string ConfirmTextSecond { get; set; }
    public string CancelText { get; set; }
    public Action ConfirmCallback { get; set; }
    public Action ConfirmCallbackSecond { get; set; }
    public Action CancelCallback { get; set; }
}
