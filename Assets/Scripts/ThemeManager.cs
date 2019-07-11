using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public List<GameObject> myList = new List<GameObject>();
    public List<Graphic> graphicList;

    private void Start()
    {
        foreach (var item in myList)
        {
            Debug.Log(item.name);
        }
    }
}
