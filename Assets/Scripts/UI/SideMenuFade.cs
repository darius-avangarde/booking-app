using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuFade : MonoBehaviour
{
    [SerializeField]
    private ThemeManager theme;
    [SerializeField]
    private List<GameObject> items = new List<GameObject>();
    [SerializeField]
    private float fadeTime = 0.35f;
    [SerializeField]
    private RectTransform sideMenu;
    private Vector2 firstPosition;
    [SerializeField]
    private GameObject sideMenuPanel;
    [SerializeField]
    private Image img;
    
    
    void Start()
    {
        firstPosition =  new Vector2(-431, 0);  //sideMenu.anchoredPosition; 
    }
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(Move(sideMenu, new Vector2(0, 0)));
        img.color = new Color(0, 0, 0, 0.5f);
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Move(sideMenu, firstPosition));
        StartCoroutine(Close());
        StartCoroutine(CanvasFade(true));
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

    IEnumerator CanvasFade(bool fadeAway)
    {
        if (fadeAway)
        {
            for (float i = 0.5f; i >= 0; i -= Time.deltaTime)
            {
                img.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 0.5f; i <= 1; i +=Time.deltaTime)
            {
                img.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }

   public void ShowMenu()
    {
        sideMenuPanel.SetActive(true);
        FadeIn();
    }
}
