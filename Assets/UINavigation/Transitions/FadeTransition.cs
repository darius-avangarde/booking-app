using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeTransition", menuName = "UINavigation/Fade Transition")]
public class FadeTransition : TransitionBase
{
    [Range(0.01f, 0.2f)]
    public float speed = 0.08f;

    public override IEnumerator Play(NavScreen currentScreen, NavScreen nextScreen)
    {
        nextScreen.RectTransform.SetAsLastSibling();
        nextScreen.gameObject.SetActive(true);
        nextScreen.OnShowing();

        if (currentScreen != null)
        {
            currentScreen.OnHiding();
            SetCanvasGroups(currentScreen.CanvasGroups, false, false);
        }

        float progress = 0f;
        do
        {
            progress += speed;
            progress = Mathf.Clamp01(progress);

            UpdateScreenFade(nextScreen.CanvasGroups, 0f, 1f, progress);

            yield return null;
        } while (progress < 1f);

        nextScreen.OnShown();
        SetCanvasGroups(nextScreen.CanvasGroups, true, true);

        if (currentScreen != null)
        {
            currentScreen.OnHidden();
            currentScreen.gameObject.SetActive(false);
            SetCanvasAlpha(currentScreen.CanvasGroups, 0);
        }
    }

    public override IEnumerator PlayReverse(NavScreen currentScreen, NavScreen previousScreen)
    {
        Handheld.StartActivityIndicator();

        currentScreen.OnHiding();
        SetCanvasGroups(currentScreen.CanvasGroups, false, false);

        if (previousScreen != null)
        {
            var currentScreenIndex = currentScreen.RectTransform.GetSiblingIndex();
            previousScreen.RectTransform.SetSiblingIndex(currentScreenIndex);
            previousScreen.gameObject.SetActive(true);
            previousScreen.OnShowing();
            SetCanvasAlpha(previousScreen.CanvasGroups, 1f);
        }

        float progress = 0f;
        do
        {
            progress += speed;
            progress = Mathf.Clamp01(progress);

            UpdateScreenFade(previousScreen.CanvasGroups, 0f, 1f, progress);

            yield return null;
        } while (progress < 1f);

        currentScreen.OnHidden();
        currentScreen.gameObject.SetActive(false);
        if (previousScreen != null)
        {
            previousScreen.OnShown();
            SetCanvasGroups(previousScreen.CanvasGroups, true,true);
        }

        yield return new WaitForEndOfFrame();
        Handheld.StopActivityIndicator();
    }

    private void UpdateScreenFade(List<CanvasGroup> canvasGroups, float from, float to, float progress)
    {
        float easedProgress = Mathf.SmoothStep(from, to, progress);
        foreach(CanvasGroup c in canvasGroups)
        {
            c.alpha = easedProgress;
        }
    }

    private void SetCanvasGroups(List<CanvasGroup> canvasGroups, bool interactable, bool blocksRaycasts)
    {
        foreach(CanvasGroup c in canvasGroups)
        {
            c.interactable = true;
            c.blocksRaycasts = true;
        }
    }

    private void SetCanvasAlpha(List<CanvasGroup> canvasGroups, float alpha)
    {
        foreach(CanvasGroup c in canvasGroups)
        {
            c.alpha = alpha;
        }
    }
}
