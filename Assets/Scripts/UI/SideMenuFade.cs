using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuFade : MonoBehaviour
{
    [SerializeField]
    private float fadeTime = 0.35f;
    [SerializeField]
    private RectTransform sideMenu;
    private Vector2 firstPosition;
    [SerializeField]
    private GameObject sideMenuPanel;

    void Start()
    {
        firstPosition =  new Vector2(-431, 0);  //sideMenu.anchoredPosition; 
    }
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(Move(sideMenu, new Vector2(0, 0)));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Move(sideMenu, firstPosition));
        StartCoroutine(Close());
    }
    IEnumerator Move(RectTransform rt, Vector2 targetPos)
    {
        float step = 0;
        while (step < 1)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, step += Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
        sideMenuPanel.SetActive(false);
    }
}
