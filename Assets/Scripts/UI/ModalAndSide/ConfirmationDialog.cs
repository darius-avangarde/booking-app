using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
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
        messageText.text = defaultMessage;
        confirmButtonText.text = defaultConfirmButtonText;
        cancelButtonText.text = defaultCancelButtonText;
    }

    public void Show(Action confirmCallback, Action cancelCallback = null)
    {
        ConfirmCallback = confirmCallback;
        CancelCallback = cancelCallback;
        easyTween.OpenCloseObjectAnimation();
    }

    public void Show(ConfirmationDialogOptions options)
    {
        messageText.text = options.Message ?? defaultMessage;
        confirmButtonText.text = options.ConfirmText ?? defaultConfirmButtonText;
        cancelButtonText.text = options.CancelText ?? defaultCancelButtonText;
        ConfirmCallback = options.ConfirmCallback;
        CancelCallback = options.CancelCallback;
        easyTween.OpenCloseObjectAnimation();
    }

    public void Confirm()
    {
        ConfirmCallback?.Invoke();

        Reset();

        easyTween.OpenCloseObjectAnimation();
    }

    public void Cancel()
    {
        CancelCallback?.Invoke();

        Reset();

        easyTween.OpenCloseObjectAnimation();
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
