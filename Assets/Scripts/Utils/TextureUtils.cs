using UnityEngine;

public static class TextureUtils
{
    public enum ImageFilterMode : int
    {
        Nearest = 0,
        Biliner = 1,
        Average = 2
    }


    public static void GeneratePropertyImages(Texture2D texture, Rect headerSize, Texture2D headerTexture, Rect fullRect, out Texture2D headerSmall, out Texture2D headerLarge, out Texture2D blurredBackground)
    {
        Color[] pixels = texture.GetPixels();

        headerSmall = CropTexture(pixels, texture, (int)headerSize.width, (int)headerSize.height);

        headerLarge = CropTexture(pixels, texture, (int)headerSize.width, (int)(headerSize.width * AspectHW(headerTexture.height, headerTexture.width)));

        blurredBackground = CropTexture(pixels, texture, 256, (int)(256 * AspectHW(fullRect.height,fullRect.width)));
        blurredBackground.SetPixels(BlurColors(blurredBackground, 10));
        blurredBackground.Apply();
    }

    public static Texture2D CropTexture(Color[] colors, Texture2D source, int targetWidth = 1080, int targetHeight = 1080)
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
        tex.SetPixels(data2);
        tex.Apply(true);
        return tex;
    }

    private static float AspectHW(float height, float width)
    {
        return height/width;
    }

    ///<summary>
    ///Optional: ImageFilterMode (nearest ,bilinear, average), max image width for downsizing, blur iterations.
    ///<para></para>
    ///<returns> downsized and blurred copy of the given Texture2D</returns>
    ///</summary>
    public static Texture2D ResizeAndBlur(Texture2D pSource, bool doBlur = true, int maxWidth = 256, int iterations = 10, ImageFilterMode pFilterMode = ImageFilterMode.Average)
    {
        //*** Variables
        int i;

        //*** Get All the source pixels
        Color[] aSourceColor = pSource.GetPixels(0);
        Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

        //*** Calculate New Size
        float xWidth  = 0;
        float xHeight = 0;

        if(pSource.width > maxWidth)
        {
            float ratio = (float)pSource.height/pSource.width;
            xHeight = maxWidth * ratio;
            xWidth = maxWidth;
        }
        else
        {
            xWidth  = pSource.width;
            xHeight = pSource.height;
        }

        //*** Make New
        Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = (int)xWidth * (int)xHeight;
        Color[] aColor = new Color[xLength];

        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

        //*** Loop through destination pixels and process
        Vector2 vCenter = new Vector2();
        for(i=0; i<xLength; i++)
        {

            //*** Figure out x&y
            float xX = (float)i % xWidth;
            float xY = Mathf.Floor((float)i / xWidth);

            //*** Calculate Center
            vCenter.x = (xX / xWidth) * vSourceSize.x;
            vCenter.y = (xY / xHeight) * vSourceSize.y;

            //*** Do Based on mode
            //*** Nearest neighbour (testing)
            if (pFilterMode == ImageFilterMode.Nearest)
            {

                //*** Nearest neighbour (testing)
                vCenter.x = Mathf.Round(vCenter.x);
                vCenter.y = Mathf.Round(vCenter.y);

                //*** Calculate source index
                int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

                //*** Copy Pixel
                aColor[i] = aSourceColor[xSourceIndex];
            }

            //*** Bilinear
            else if (pFilterMode == ImageFilterMode.Biliner)
            {

                //*** Get Ratios
                float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
                float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

                //*** Get Pixel index's
                int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
                int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

                //*** Calculate Color
                aColor[i] = Color.Lerp(
                    Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
                    Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
                    xRatioY
                );
            }

            //*** Average
            //*** Average
            else if (pFilterMode == ImageFilterMode.Average)
            {

                //*** Calculate grid around point
                int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
                int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
                int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
                int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

                //*** Loop and accumulate
                Color oColorTemp = new Color();
                float xGridCount = 0;
                for (int iy = xYFrom; iy < xYTo; iy++)
                {
                    for (int ix = xXFrom; ix < xXTo; ix++)
                    {

                        //*** Get Color
                        oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                        //*** Sum
                        xGridCount++;
                    }
                }

                //*** Average Color
                aColor[i] = oColorTemp / (float)xGridCount;
            }
        }

        if(doBlur)
        {
            aColor = BlurColors(oNewTex, aColor, iterations);
        }

        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();

        //*** Return
        return oNewTex;
    }

    ///<summary>
    ///Returns a center cropped copy of the given Texture2D of the given width and height.
    ///</summary>
    public static Texture2D CropTexture(Texture2D pSource, int targetWidth = 1080, int targetHeight = 1080)
    {
        Texture2D source = pSource;

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
        Color[] data = source.GetPixels();
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
        tex.SetPixels(data2);
        tex.Apply(true);
        return tex;
    }

    private static Color[] BlurColors(Texture2D pSource, Color[] colorsToBlur, int iterations)
    {
            Color startcol = Color.green;
            Color endcol = Color.red;
            Color sidecol = Color.yellow;


        bool isTall = pSource.height > pSource.width;

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

    private static Color[] BlurColors(Texture2D pSource, int iterations)
    {
        Color[] colorsToBlur = pSource.GetPixels(0);


        bool isTall = pSource.height > pSource.width;

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

    //get color data
    // > crop and return >
        // small
        // big
        // bg + blur
    //

    //crop and return (colordata, texure, blurt iterations = 0 [if 0 dont])
        // rez math
        // crop func
            // if it > 0
                // apply color blur
        // return texture


    //rewrite to get color data at first then resize and return textures separately, but use same color data
    //blur using same color data (convert to use c32)
}
