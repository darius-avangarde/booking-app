using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class TestEdit : MonoBehaviour
{
    public Sprite replaceSprite;
    public Sprite defaultHeader;
    public RectTransform smallHeaderTransform;
    public RectTransform fullRect;

    public Image headerSmall;
    public Image headerLarge;
    public Image blurredBackground;


    [ContextMenu("Resize images")]
    public void ResizeTargetImage()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Texture2D smallHead;
        Texture2D largeHead;
        Texture2D blurred;

        TextureUtils.GeneratePropertyImages(replaceSprite.texture, smallHeaderTransform.rect, defaultHeader.texture, fullRect.rect, out smallHead, out largeHead, out blurred);

        Sprite small = Sprite.Create(smallHead, new Rect(0, 0, smallHead.width, smallHead.height), new Vector2(0.5f, 0.5f));
        Sprite large = Sprite.Create(largeHead, new Rect(0, 0, largeHead.width, largeHead.height), new Vector2(0.5f, 0.5f));
        Sprite blurBG = Sprite.Create(blurred, new Rect(0, 0, blurred.width, blurred.height), new Vector2(0.5f, 0.5f));

        headerSmall.sprite = small;
        headerLarge.sprite = large;
        blurredBackground.sprite = blurBG;

        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.Elapsed);
    }

    [ContextMenu("RESET sprites")]
    public void resetSprites()
    {
        headerSmall.sprite = replaceSprite;
        headerLarge.sprite = replaceSprite;
        blurredBackground.sprite = replaceSprite;
    }
}
