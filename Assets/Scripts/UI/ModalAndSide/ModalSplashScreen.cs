using System.Collections;
using UnityEngine;

public class ModalSplashScreen : MonoBehaviour, IClosable
{
    [SerializeField]
    private CanvasGroup splashScreenCanvasGroup;
    [SerializeField]
    private CanvasGroup blackFadeCanvasGroup;
    [SerializeField]
    private float splashFadeTime = 0.5f;
    [SerializeField]
    private float blackFadeTime = 0.5f;

    private bool canDismiss = false;

    private void Start()
    {
        canDismiss = false;
        InputManager.CurrentlyOpenClosable = this;
        StartCoroutine(ClearBlack());
    }

    public void Close()
    {
        if(canDismiss)
        {
            canDismiss = false;
            StopAllCoroutines();
            StartCoroutine(ClearSplash());
            InputManager.CurrentlyOpenClosable = null;
        }
    }

    private IEnumerator ClearBlack()
    {

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/blackFadeTime)
        {
            blackFadeCanvasGroup.alpha = Mathf.Lerp(1,0,t);
            yield return null;
        }
        blackFadeCanvasGroup.alpha = 0;
        splashScreenCanvasGroup.interactable = true;
        canDismiss = true;

        StartCoroutine(WaitOne());
    }

    private IEnumerator WaitOne()
    {
        yield return new WaitForSeconds(1);
        Close();
    }

    private IEnumerator ClearSplash()
    {
        splashScreenCanvasGroup.interactable = false;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/splashFadeTime)
        {
            splashScreenCanvasGroup.alpha = Mathf.Lerp(1,0,t);
            yield return null;
        }
        splashScreenCanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}
