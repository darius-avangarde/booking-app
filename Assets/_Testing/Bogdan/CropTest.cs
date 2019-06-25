using UnityEngine;
using UnityEngine.UI;

public class CropTest : MonoBehaviour
{
    [SerializeField]
    Sprite defaultSprite;

    [SerializeField]
    Image blurredImage;
    [SerializeField]
    Image headerImage;

    [SerializeField]
    RectTransform blurredImageRect;
    [SerializeField]
    RectTransform headerImageRect;

    [ContextMenu("RESET")]
    private void ResetSprites()
    {
        blurredImage.sprite = defaultSprite;
        headerImage.sprite = defaultSprite;
    }


    [ContextMenu("CROP AND BLUR")]
    private void CropNblur()
    {
        // ResetSprites();

        // Texture2D h;
        // Texture2D b;

        // TextureUtils.GeneratePropertyTextures(defaultSprite.texture, headerImageRect.rect, blurredImageRect.rect, out h, out b);

        // Sprite H = Sprite.Create(h, new Rect(0, 0, h.width, h.height), new Vector2(0.5f, 0.5f));
        // Sprite B = Sprite.Create(b, new Rect(0, 0, b.width, b.height), new Vector2(0.5f, 0.5f));

        // headerImage.sprite = H;
        // blurredImage.sprite = B;
    }
}
