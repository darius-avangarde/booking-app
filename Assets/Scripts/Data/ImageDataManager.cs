using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageDataManager
{
    public const string DATA_FILE_NAME = "ImageData.json";
    public static bool AddedPhoto;
    public static Hashtable PropertyPhotos = new Hashtable();
    public static Hashtable BlurPropertyPhotos = new Hashtable();
    private static string PropertyPhotosFolder = Path.Combine(Application.persistentDataPath, "PropertyPhotos");

    /// <summary>
    /// pick an image from galery
    /// </summary>
    /// <param name="propertyID">id of the current property</param>
    /// <param name="propertyImage">header image of the property admin screen</param>
    /// <param name="backgroundImage">backgroun image of property admin screen</param>
    /// <param name="imageAspectRatio">aspect ratio fitter of header image</param>
    /// <param name="backgroundAspectRatio">aspect ratio fitter of background image</param>
    public static void PickImage(string propertyID, Image propertyImage, Image backgroundImage, AspectRatioFitter imageAspectRatio, AspectRatioFitter backgroundAspectRatio)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            //Debug.Log("Image path: " + path);
            if (!string.IsNullOrEmpty(path))
            {
                AddedPhoto = true;
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1080, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                propertyImage.sprite = downloadedImage;
                imageAspectRatio.aspectRatio = (float)propertyImage.sprite.texture.width / propertyImage.sprite.texture.height;

                Texture2D blurredImage = TextureUtils.ResizeAndBlur(texture);
                Sprite downloadedBlurredImage = Sprite.Create(blurredImage, new Rect(0, 0, blurredImage.width, blurredImage.height), new Vector2(0.5f, 0.5f));
                backgroundImage.sprite = downloadedBlurredImage;
                backgroundAspectRatio.aspectRatio = (float)backgroundImage.sprite.texture.width / backgroundImage.sprite.texture.height;
            }
            else
            {
                AddedPhoto = false;
            }
        }, "Select an image");
        //Debug.Log("Permission result: " + permission);
    }

    /// <summary>
    /// take a photo using the camera app
    /// </summary>
    /// <param name="propertyID">id of the current property</param>
    /// <param name="propertyImage">header image of the property admin screen</param>
    /// <param name="backgroundImage">backgroun image of property admin screen</param>
    /// <param name="imageAspectRatio">aspect ratio fitter of header image</param>
    /// <param name="backgroundAspectRatio">aspect ratio fitter of background image</param>
    public static void TakePhoto(string propertyID, Image propertyImage, Image backgroundImage, AspectRatioFitter imageAspectRatio, AspectRatioFitter backgroundAspectRatio)
    {
        AddedPhoto = false;
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            //Debug.Log("Image path: " + path);
            if (!string.IsNullOrEmpty(path))
            {
                AddedPhoto = true;
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, 1080, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                propertyImage.sprite = downloadedImage;
                imageAspectRatio.aspectRatio = (float)propertyImage.sprite.texture.width / propertyImage.sprite.texture.height;

                Texture2D blurredImage = TextureUtils.ResizeAndBlur(texture);
                Sprite downloadedBlurredImage = Sprite.Create(blurredImage, new Rect(0, 0, blurredImage.width, blurredImage.height), new Vector2(0.5f, 0.5f));
                backgroundImage.sprite = downloadedBlurredImage;
                backgroundAspectRatio.aspectRatio = (float)backgroundImage.sprite.texture.width / backgroundImage.sprite.texture.height;
            }
            else
            {
                AddedPhoto = false;
            }
        });
        //Debug.Log("Permission result: " + permission);
    }

    /// <summary>
    /// save current image to persistent data path
    /// </summary>
    /// <param name="propertyID">id of the current property</param>
    /// <param name="propertyImage">refference to the texture saved on header</param>
    /// <param name="backgroundImage">refference to the texture saved on background</param>
    public static void SaveImage(string propertyID, Texture2D propertyImage, Texture2D backgroundImage)
    {
        string filePath = Path.Combine(PropertyPhotosFolder, propertyID + ".png");
        string blurFilePath = Path.Combine(PropertyPhotosFolder, propertyID + "_blur.png");
        DirectoryInfo dirInf = new DirectoryInfo(PropertyPhotosFolder);
        if (!dirInf.Exists)
        {
            Directory.CreateDirectory(PropertyPhotosFolder);
        }
        File.WriteAllBytes(filePath, propertyImage.EncodeToPNG());
        Sprite downloadedImage = Sprite.Create(propertyImage, new Rect(0, 0, propertyImage.width, propertyImage.height), new Vector2(0.5f, 0.5f));
        PropertyPhotos[propertyID] = downloadedImage;

        File.WriteAllBytes(blurFilePath, backgroundImage.EncodeToPNG());
        Sprite downloadedImageBlur = Sprite.Create(backgroundImage, new Rect(0, 0, backgroundImage.width, backgroundImage.height), new Vector2(0.5f, 0.5f));
        BlurPropertyPhotos[propertyID] = downloadedImageBlur;
    }

    /// <summary>
    /// load all images from persistent data path and save them to hashtables
    /// </summary>
    public static void LoadAllPropertyImages()
    {
        Texture2D defaultPicture = Resources.Load(Constants.defaultPropertyPicture) as Texture2D;
        PropertyPhotos[Constants.defaultPropertyPicture] = Sprite.Create(defaultPicture, new Rect(0, 0, defaultPicture.width, defaultPicture.height), new Vector2(0.5f, 0.5f));
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            if (GetPropertyPhoto(property.ID, false) != null && !PropertyPhotos.ContainsKey(property.ID))
            {
                PropertyPhotos[property.ID] = GetPropertyPhoto(property.ID, false);
            }
            if (GetPropertyPhoto(property.ID, true) != null && !BlurPropertyPhotos.ContainsKey(property.ID))
            {
                BlurPropertyPhotos[property.ID] = GetPropertyPhoto(property.ID, true);
            }
        }
    }

    /// <summary>
    /// get the saved photo for a specific property
    /// the propertyID string is the name of the photo
    /// </summary>
    /// <param name="propertyID">id of the current property</param>
    /// <param name="blur">select the blurred photo or the normal one</param>
    /// <returns></returns>
    private static Sprite GetPropertyPhoto(string propertyID, bool blur)
    {
        string filePath;
        if (blur)
        {
            filePath = Path.Combine(PropertyPhotosFolder, propertyID + "_blur.png");
        }
        else
        {
            filePath = Path.Combine(PropertyPhotosFolder, propertyID + ".png");
        }
        DirectoryInfo dirInf = new DirectoryInfo(PropertyPhotosFolder);
        if (!dirInf.Exists)
        {
            Directory.CreateDirectory(PropertyPhotosFolder);
        }
        if (File.Exists(filePath))
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(filePath, 1080, false);
            Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            return downloadedImage;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// delete saved photos for a certain property
    /// </summary>
    /// <param name="propertyID">the id of the current property</param>
    public static void DeletePropertyPhoto(string propertyID)
    {
        string filePath = Path.Combine(PropertyPhotosFolder, propertyID + ".png");
        string blurFilePath = Path.Combine(PropertyPhotosFolder, propertyID + "_blur.png");
        DirectoryInfo dirInf = new DirectoryInfo(PropertyPhotosFolder);
        if (!dirInf.Exists)
        {
            Directory.CreateDirectory(PropertyPhotosFolder);
        }
        if (File.Exists(filePath))
        {
            PropertyPhotos.Remove(propertyID);
            File.Delete(filePath);
        }
        if (File.Exists(blurFilePath))
        {
            BlurPropertyPhotos.Remove(propertyID);
            File.Delete(blurFilePath);
        }
    }
}
