using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntegerSelector : MonoBehaviour
{
    [SerializeField]
    private Text text = null;

    public IntUnityEvent OnChangeValue;

    private Color enabledColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);

    private int value = 0;
    public int Value
    {
        get => value;
        set
        {
            this.value = value < 0 ? 0 : value;
            text.color = this.value > 0 ? enabledColor : disabledColor;
            text.text = this.value.ToString();
            OnChangeValue.Invoke(this.value);
        }
    }

    public void Increment()
    {
        Value++;
    }

    public void Decrement()
    {
        Value--;
    }

    public void SetToZero()
    {
        Value = 0;
    }
}
