using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportPhotos : MonoBehaviour
{
    public Image testImage;

    public void PickImage(Image testImage)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
        //Debug.Log("Image path: " + path);
            if (!string.IsNullOrEmpty(path))
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                testImage.sprite = downloadedImage;
            }
        }, "Select an image");

        //Debug.Log("Permission result: " + permission);
    }

    public void TakePhoto(Image testImage)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            //Debug.Log("Image path: " + path);
            if (!string.IsNullOrEmpty(path))
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                testImage.sprite = downloadedImage;
            }
        });

        //Debug.Log("Permission result: " + permission);
    }
}
