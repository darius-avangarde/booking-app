using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddBtnScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onClick;

    public void ButtonAction()
    {
        onClick?.Invoke();
    }
}
