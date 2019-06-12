using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageDataManager
{
    public const string DATA_FILE_NAME = "ImageData.json";
    public static Hashtable PropertyPhotos = new Hashtable();
    private static string PropertyPhotosFolder = Path.Combine(Application.persistentDataPath, "PropertyPhotos");

    public static void PickImage(string propertyID, Image propertyImage)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            //Debug.Log("Image path: " + path);
            if (!string.IsNullOrEmpty(path))
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1080, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                propertyImage.sprite = downloadedImage;
            }
        }, "Select an image");
        //Debug.Log("Permission result: " + permission);
    }

    public static void TakePhoto(string propertyID, Image propertyImage)
    {
        Sprite downloadedImage = null;
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            //Debug.Log("Image path: " + path);
            if (!string.IsNullOrEmpty(path))
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, 1080, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                propertyImage.sprite = downloadedImage;
            }
        });
        //Debug.Log("Permission result: " + permission);
    }

    public static void SaveImage(string propertyID, Texture2D propertyImage)
    {
        string filePath = Path.Combine(PropertyPhotosFolder, propertyID + ".png");
        DirectoryInfo dirInf = new DirectoryInfo(PropertyPhotosFolder);
        if (!dirInf.Exists)
        {
            Directory.CreateDirectory(PropertyPhotosFolder);
        }
        File.WriteAllBytes(filePath, propertyImage.EncodeToPNG());

        Sprite downloadedImage = Sprite.Create(propertyImage, new Rect(0, 0, propertyImage.width, propertyImage.height), new Vector2(0.5f, 0.5f));
        PropertyPhotos[propertyID] = downloadedImage;
    }

    public static void LoadAllPropertyImages()
    {
        Texture2D defaultPicture = Resources.Load(Constants.defaultPropertyPicture) as Texture2D;
        PropertyPhotos[Constants.defaultPropertyPicture] = Sprite.Create(defaultPicture, new Rect(0, 0, defaultPicture.width, defaultPicture.height), new Vector2(0.5f, 0.5f));
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            if (GetPropertyPhoto(property.ID) != null && !PropertyPhotos.ContainsKey(property.ID))
            {
                PropertyPhotos[property.ID] = GetPropertyPhoto(property.ID);
            }
        }
    }

    private static Sprite GetPropertyPhoto(string propertyID)
    {
        string filePath = Path.Combine(PropertyPhotosFolder, propertyID + ".png");
        DirectoryInfo dirInf = new DirectoryInfo(PropertyPhotosFolder);
        if (!dirInf.Exists)
        {
            Directory.CreateDirectory(PropertyPhotosFolder);
        }
        if (File.Exists(filePath))
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(filePath, 1080);
            Sprite downloadedImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            return downloadedImage;
        }
        else
        {
            return null;
        }
    }

    public static void DeletePropertyPhoto(string propertyID)
    {
        string filePath = Path.Combine(PropertyPhotosFolder, propertyID + ".png");
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
    }
}