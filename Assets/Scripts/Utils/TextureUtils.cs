using UnityEngine;

public static class TextureUtils
{
    public enum ImageFilterMode : int
    {
        Nearest = 0,
        Biliner = 1,
        Average = 2
    }

    ///<summary>
    ///Optional: ImageFilterMode (nearest ,bilinear, average), max image width for downsizing, blur iterations.
    ///<para></para>
    ///<returns> downsized and blurred copy of the given Texture2D</returns>
    ///</summary>
    public static Texture2D ResizeAndBlur(Texture2D pSource, ImageFilterMode pFilterMode = ImageFilterMode.Average, int maxWidth = 256, int iterations = 10)
    {
        //*** Variables
        int i;

        //*** Get All the source pixels
        Color[] aSourceColor = pSource.GetPixels(0);
        Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

        //*** Calculate New Size
        float xWidth  = 0;
        float xHeight = 0;

        if(Mathf.Max(pSource.height, pSource.width) > maxWidth)
        {
            float ratio = 0;

            ratio = (float)pSource.height/pSource.width;
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

        aColor = BlurColors(oNewTex, aColor, iterations);

        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();

        //*** Return
        return oNewTex;
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
}
