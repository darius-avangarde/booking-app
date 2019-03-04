using UnityEngine;
using UnityEngine.UI;

public class SideMenuButton : MonoBehaviour
{
    [SerializeField]
    private Button button = null;
    [SerializeField]
    private EasyTween backgroundColor1 = null;
    [SerializeField]
    private EasyTween backgroundColor2 = null;

    private bool active = false;

    public bool Active
    {
        get => active;
        set {
            if (value != active)
            {
                button.interactable = false;
                backgroundColor1.OpenCloseObjectAnimation();
                backgroundColor2.OpenCloseObjectAnimation();
                active = value;
            }
        }
    }

    public void Toggle()
    {
        Active = !Active;
    }
}
