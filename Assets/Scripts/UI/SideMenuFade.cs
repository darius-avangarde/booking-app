using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuFade : MonoBehaviour, IClosable
{
    [SerializeField]
    private RectTransform sideMenu;
    private Vector2 firstPosition;
    [SerializeField]
    private GameObject sideMenuPanel;
    [SerializeField]
    private CanvasGroup sideMenuCanvasGroup;
    [SerializeField]
    private Image img;
    private Color startColor = new Color(0, 0, 0, 0.5f);
    private Color finalColor = new Color(0, 0, 0, 0f);

    void Start()
    {
        firstPosition = new Vector2(-650, 0); 
    }
    public void FadeIn()
    {
        sideMenuCanvasGroup.interactable = true;
        StopAllCoroutines();
        StartCoroutine(Move(sideMenu, new Vector2(0, 0),false));
        img.color = new Color(0, 0, 0, 0.5f);
    }

    public void FadeOut()
    {
        sideMenuCanvasGroup.interactable = false;
        StopAllCoroutines();
        StartCoroutine(Move(sideMenu, firstPosition, true));
        StartCoroutine(CloseMenu());
        
    }
    IEnumerator Move(RectTransform rt, Vector2 targetPos, bool fadeAway)
    {
        float step = 0;
        while (step < 1)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, step += Time.deltaTime);
            if (fadeAway)
            {
                img.color = finalColor;
            }
            else
            {
                img.color = startColor;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(0.5f);
        sideMenuPanel.SetActive(false);
    }

    public void ShowMenu()
    {
        sideMenuPanel.SetActive(true);
        FadeIn();
    }

    public void Close()
    {
        FadeOut();
    }
}
