using UnityEngine;

public static class TextureUtils
{
    //TODO: REMOVE THIS
    public static Texture2D ResizeAndBlur(Texture2D remove)
    {
        return null;
    }


    ///<summary>
    ///Outputs two cropped copies of the given texture sizing it to the given rects, blurredBackground texture is also blurred.
    ///</summary>
    public static void GeneratePropertyTextures(Texture2D texture, Rect headerSize, Rect fullRect, out Texture2D headerImage, out Texture2D blurredBackground)
    {
        Color[] pixels = texture.GetPixels();

        headerImage = CropTexture(pixels, texture, (int)fullRect.width, (int)(headerSize.height));
        blurredBackground = CropTexture(pixels, texture, 256, (int)(256 * AspectHW(fullRect.height,fullRect.width)), true);
    }

    public static Texture2D CropTexture(Color[] colors, Texture2D source, int targetWidth = 1080, int targetHeight = 1080, bool doBlur = false)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        float sourceAspect = (float)sourceWidth / sourceHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;

        if (sourceAspect > targetAspect)
        { // crop width
            factor = (float)targetHeight / sourceHeight;
            xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
        }
        else
        { // crop height
            factor = (float)targetWidth / sourceWidth;
            yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
        }

        Color[] data = colors;
        Color[] data2 = new Color[targetWidth * targetHeight];
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                Vector2 p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                // bilinear filtering
                Color c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                Color c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                Color c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                Color c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                Vector2 f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
            }
        }

        Texture2D tex = new Texture2D(targetWidth, targetHeight);

        if(doBlur)
        {
            tex.SetPixels(BlurColors(tex, data2, 10));
        }
        else
        {
            tex.SetPixels(data2);
        }

        tex.Apply(true);
        return tex;
    }

    private static Color[] BlurColors(Texture2D pSource, Color[] colorsToBlur, int iterations)
    {
        Color[] aSourceColor = colorsToBlur;
        Color[,] colors = new Color[pSource.width, pSource.height];
        Color[,] passColors = new Color[pSource.width, pSource.height];
        Color[] newColors = new Color[pSource.width * pSource.height];

        //Map 2D
        for (int c = 0; c < aSourceColor.Length; c++)
        {
            colors[c % pSource.width, c / pSource.width] = aSourceColor[c];
        }

        for(int i = 0; i < iterations; i++)
        {
            for (int x = 0; x < pSource.width; x++)
            {
                for (int y = 0; y < pSource.height; y++)
                {

                    int leftX   = Mathf.Clamp(x - 1, 0, pSource.width - 1);
                    int rightX  = Mathf.Clamp(x + 1, 0, pSource.width - 1);
                    int upperY  = Mathf.Clamp(y + 1, 0, pSource.height - 1);
                    int lowerY  = Mathf.Clamp(y - 1, 0, pSource.height - 1);

                    Color upperRow  = colors[leftX, upperY]
                                    + colors[x, upperY]
                                    + colors[rightX, upperY];

                    Color centerRow = colors[leftX, y]
                                    + colors[x, y]
                                    + colors[rightX, y];

                    Color bottomRow = colors[leftX, lowerY]
                                    + colors[x, lowerY]
                                    + colors[rightX, lowerY];

                    passColors[x,y] = (upperRow + centerRow + bottomRow) / 9;
                }
            }

            colors = passColors;
        }

        //Remap 1D
        for (int x = 0; x < pSource.width; x++)
        {
            for (int y = 0; y < pSource.height; y++)
            {
                newColors[x + pSource.width * y] = colors[x,y];
            }
        }

        return newColors;
    }

    private static float AspectHW(float height, float width)
    {
        return height/width;
    }
}
