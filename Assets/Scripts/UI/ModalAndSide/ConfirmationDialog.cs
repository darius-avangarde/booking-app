using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour, IClosable
{
    [SerializeField]
    private EasyTween easyTween = null;
    [SerializeField]
    private Text messageText = null;
    [SerializeField]
    private Text cancelButtonText = null;
    [SerializeField]
    private Text confirmButtonText = null;

    public string defaultMessage = "Sunteți sigur?";
    public string defaultConfirmButtonText = "Confirmați";
    public string defaultCancelButtonText = "Anulați";

    private Action ConfirmCallback;
    private Action CancelCallback;

    private void Reset()
    {
        ConfirmCallback = null;
        CancelCallback = null;
    }

    public void Show(ConfirmationDialogOptions options)
    {
        messageText.text = options.Message ?? defaultMessage;
        confirmButtonText.text = options.ConfirmText ?? defaultConfirmButtonText;
        cancelButtonText.text = options.CancelText ?? defaultCancelButtonText;
        ConfirmCallback = options.ConfirmCallback;
        CancelCallback = options.CancelCallback;
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = this;
    }

    public void Confirm()
    {
        ConfirmCallback?.Invoke();
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = null;
        Reset();
    }

    public void Cancel()
    {
        CancelCallback?.Invoke();
        easyTween.OpenCloseObjectAnimation();
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
    public string Message { get; set; }
    public string ConfirmText { get; set; }
    public string CancelText { get; set; }
    public Action ConfirmCallback { get; set; }
    public Action CancelCallback { get; set; }
}
