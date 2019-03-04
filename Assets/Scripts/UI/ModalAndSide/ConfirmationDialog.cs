using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
    public EasyTween easyTween;
    [SerializeField]
    private Text message;
    [SerializeField]
    private Text cancelButtonText;
    [SerializeField]
    private Text confirmButtonText;

    private Action ConfirmCallback;
    private Action CancelCallback;

    public void ShowTest() {easyTween.OpenCloseObjectAnimation();}

    public void Show(Action confirmCallback, Action cancelCallback)
    {
        easyTween.OpenCloseObjectAnimation();
        ConfirmCallback = confirmCallback;
        CancelCallback = cancelCallback;
    }

    public void Confirm()
    {
        ConfirmCallback?.Invoke();

        ConfirmCallback = null;
        CancelCallback = null;

        easyTween.OpenCloseObjectAnimation();
    }

    public void Cancel()
    {
        CancelCallback?.Invoke();

        ConfirmCallback = null;
        CancelCallback = null;

        easyTween.OpenCloseObjectAnimation();
    }
}
