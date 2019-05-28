using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalDayObject : MonoBehaviour
{
    [SerializeField]
    private Image startImage;
    [SerializeField]
    private Image endImage;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Button buttonComp;
    [SerializeField]
    private Text dayObjText;

    private DateTime objDayTime;

    private bool isStart;
    private bool isEnd;
    private bool isReserved;

    internal bool IsStart
    {
        get => isStart;
        set => isStart = value;
    }

    internal bool IsEnd
    {
        get => isEnd;
        set => isEnd = value;
    }

    internal bool IsReserved
    {
        get => isReserved;
        set => isReserved = value;
    }

    internal DateTime ObjDate => objDayTime;

    internal void SetListener(UnityAction<ModalDayObject> clickAction)
    {
        buttonComp.onClick.AddListener(() => clickAction(this));
    }

    internal void UpdateDayObject(DateTime dateTime)
    {
        objDayTime = dateTime;
        dayObjText.text = dateTime.Day.ToString();
    }

    internal void UpdateDayColors(Color? bg, Color? start, Color? end)
    {
        backgroundImage.color = bg ?? backgroundImage.color;
        startImage.color = start ?? startImage.color;
        endImage.color = end ?? endImage.color;
    }
}
