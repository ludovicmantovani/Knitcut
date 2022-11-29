using System.Collections.Generic;
using UnityEngine;

public class GestureClass
{
    public List<Vector3> mouseData = new List<Vector3>();

    private bool gesturing;

    public int widthText = 3;

    // OLD -> To remove
    public Texture2D MapPattern()
    {
        Bounds bounds = new Bounds(mouseData[0], Vector3.zero);
        for (int i = 1; i < mouseData.Count; i++)
        {
            bounds.min = Vector3.Min(bounds.min, mouseData[i]);
            bounds.max = Vector3.Max(bounds.max, mouseData[i]);
        }

        Texture2D texture2D = new Texture2D(32, 32);
        Color[] pixels = texture2D.GetPixels();
        for (int j = 0; j < pixels.Length; j++)
        {
            ref Color reference = ref pixels[j];
            reference = Color.white;
        }

        if (bounds.size.magnitude < 20f)
        {
            return null;
        }

        for (int k = 0; k < mouseData.Count - 1; k++)
        {
            int num = (int)Mathf.Clamp((mouseData[k].x - bounds.min.x) / bounds.size.x * 32f, 0f, 31f);
            int num2 = (int)Mathf.Clamp((mouseData[k].y - bounds.min.y) / bounds.size.y * 32f, 0f, 31f);
            int num3 = (int)Mathf.Clamp((mouseData[k + 1].x - bounds.min.x) / bounds.size.x * 32f, 0f, 31f);
            int num4 = (int)Mathf.Clamp((mouseData[k + 1].y - bounds.min.y) / bounds.size.y * 32f, 0f, 31f);
            float num5 = Mathf.Sqrt(Mathf.Pow(num3 - num, 2f) + Mathf.Pow(num4 - num2, 2f));
            for (int l = 0; l <= 20; l++)
            {
                float num6 = (float)l * 0.05f;
                int num7 = (int)((float)num + (float)(num3 - num) * num6);
                int num8 = (int)((float)num2 + (float)(num4 - num2) * num6);
                ref Color reference2 = ref pixels[num7 + num8 * 32];
                reference2 = Color.black;
                for (int m = 1; m < widthText; m++)
                {
                    int num9 = (int)((float)num7 + (float)((num4 - num2) * m) / num5);
                    int num10 = (int)((float)num8 - (float)((num3 - num) * m) / num5);
                    int num11 = (int)((float)num7 - (float)((num4 - num2) * m) / num5);
                    int num12 = (int)((float)num8 + (float)((num3 - num) * m) / num5);
                    if (num9 >= 0 && num9 < 32 && num10 >= 0 && num10 < 32)
                    {
                        ref Color reference3 = ref pixels[num9 + num10 * 32];
                        reference3 = Color.black;
                    }

                    if (num11 >= 0 && num11 < 32 && num12 >= 0 && num12 < 32)
                    {
                        ref Color reference4 = ref pixels[num11 + num12 * 32];
                        reference4 = Color.black;
                    }
                }
            }
        }

        texture2D.SetPixels(pixels);
        texture2D.Apply();
        return texture2D;
    }

    // NEW
    public Texture2D ConvertToTexture2D(Vector2 resolution)
    {
        Bounds bounds = new Bounds(mouseData[0], Vector3.zero);
        for (int i = 1; i < mouseData.Count; i++)
        {
            bounds.min = Vector3.Min(bounds.min, mouseData[i]);
            bounds.max = Vector3.Max(bounds.max, mouseData[i]);
        }

        Texture2D texture2D = new Texture2D((int)resolution.x, (int)resolution.y);
        float resolutionLimit = resolution.x;

        Color[] pixels = texture2D.GetPixels();
        for (int j = 0; j < pixels.Length; j++)
        {
            ref Color reference = ref pixels[j];
            reference = Color.white;
        }

        if (bounds.size.magnitude < 20f)
        {
            return null;
        }

        for (int k = 0; k < mouseData.Count - 1; k++)
        {
            int num = (int)Mathf.Clamp((mouseData[k].x - bounds.min.x) / bounds.size.x * resolutionLimit, 0f, resolutionLimit-1);
            int num2 = (int)Mathf.Clamp((mouseData[k].y - bounds.min.y) / bounds.size.y * resolutionLimit, 0f, resolutionLimit - 1);
            int num3 = (int)Mathf.Clamp((mouseData[k + 1].x - bounds.min.x) / bounds.size.x * resolutionLimit, 0f, resolutionLimit - 1);
            int num4 = (int)Mathf.Clamp((mouseData[k + 1].y - bounds.min.y) / bounds.size.y * resolutionLimit, 0f, resolutionLimit - 1);
            float num5 = Mathf.Sqrt(Mathf.Pow(num3 - num, 2f) + Mathf.Pow(num4 - num2, 2f));
            for (int l = 0; l <= 20; l++)
            {
                float num6 = (float)l * 0.05f;
                int num7 = (int)((float)num + (float)(num3 - num) * num6);
                int num8 = (int)((float)num2 + (float)(num4 - num2) * num6);
                ref Color reference2 = ref pixels[num7 + num8 * (int)resolutionLimit];
                reference2 = Color.black;
                for (int m = 1; m < widthText; m++)
                {
                    int num9 = (int)((float)num7 + (float)((num4 - num2) * m) / num5);
                    int num10 = (int)((float)num8 - (float)((num3 - num) * m) / num5);
                    int num11 = (int)((float)num7 - (float)((num4 - num2) * m) / num5);
                    int num12 = (int)((float)num8 + (float)((num3 - num) * m) / num5);
                    if (num9 >= 0 && num9 < (int)resolutionLimit && num10 >= 0 && num10 < resolutionLimit)
                    {
                        ref Color reference3 = ref pixels[num9 + num10 * (int)resolutionLimit];
                        reference3 = Color.black;
                    }

                    if (num11 >= 0 && num11 < resolutionLimit && num12 >= 0 && num12 < (int)resolutionLimit)
                    {
                        ref Color reference4 = ref pixels[num11 + num12 * (int)resolutionLimit];
                        reference4 = Color.black;
                    }
                }
            }
        }

        texture2D.SetPixels(pixels);
        texture2D.Apply();
        return texture2D;
    }

    // OLD -> To remove
    public float TestPattern(Texture2D textureDrawing, Texture2D texturePattern)
    {
        if (texturePattern == null)
        {
            Debug.LogError("<b>Mouse Gesture Interpretation:</b> texture pattern for comparison is not set.");
            return 0f;
        }

        Color[] pixelsTextureDrawing = textureDrawing.GetPixels();
        Color[] pixelsTexturePattern = texturePattern.GetPixels();
        float numBlackPixelsTexturePattern = 0f;
        float numBlackPixelsTextureDrawing = 0f;
        float numSamePixelsTextures = 0f;
        float numExtraPixels = 0f;

        for (int i = 0; i < pixelsTexturePattern.Length; i++)
        {
            if (pixelsTexturePattern[i] == Color.black)
            {
                numBlackPixelsTexturePattern ++;
            }
        }

        for (int j = 0; j < pixelsTextureDrawing.Length; j++)
        {
            if (pixelsTextureDrawing[j] == Color.black)
            {
                numBlackPixelsTextureDrawing ++;

                if (pixelsTexturePattern[j] == Color.black)
                {
                    numSamePixelsTextures ++;
                }

                if (pixelsTexturePattern[j] == Color.white)
                {
                    numExtraPixels++;
                }
            }
        }

        //Debug.Log($"black pixels pattern : {numBlackPixelsTPattern} - black pixels drawing : {numBlackPixelsTDrawing} - same {numSamePixelsTextures} - extra {numExtraPixels}");

        //float num4 = numBlackPixelsTDrawing - numSamePixelsTextures;
        float result = numSamePixelsTextures / numBlackPixelsTexturePattern;
        //Debug.Log($"numBlackPixelsTPattern: {numBlackPixelsTPattern} - numBlackPixelsTDrawing: {numBlackPixelsTDrawing} - numSamePixelsTextures: {numSamePixelsTextures} - num4: {num4} - result: {result}");
        /*if (num4 < numSamePixelsTextures)
        {
            return result;
        }*/

        //return 0f;
        return result;
    }

    // NEW
    public float[] CompareDrawingWithPattern(Texture2D textureDrawing, Texture2D texturePattern)
    {
        float[] results = new float[2]; // 0:score - 1:extra

        if (texturePattern == null)
        {
            Debug.LogError("<b>Mouse Gesture Interpretation:</b> texture pattern for comparison is not set.");
            results[0] = 0f;
            results[1] = 0f;
            return results;
        }

        Color[] pixelsTDrawing = textureDrawing.GetPixels();
        Color[] pixelsTPattern = texturePattern.GetPixels();

        float numBlackPixelsTDrawing = 0f;
        float numSamePixelsTextures = 0f;
        float numExtraPixels = 0f;
        float othersPattern = 0f;

        for (int i = 0; i < pixelsTPattern.Length; i++)
        {
            if (pixelsTPattern[i] != Color.black && pixelsTPattern[i] != Color.white)
            {
                othersPattern++;
                pixelsTPattern[i] = Color.black;
            }

            if (pixelsTDrawing[i] == Color.black) numBlackPixelsTDrawing++;

            if (pixelsTPattern[i] == Color.black && pixelsTDrawing[i] == Color.black) numSamePixelsTextures++;
            if (pixelsTPattern[i] == Color.white && pixelsTDrawing[i] == Color.black) numExtraPixels++;
        }

        float percentageExtra = numExtraPixels / numBlackPixelsTDrawing;
        
        /*Debug.Log($"drawing black={numBlackPixelsTDrawing} - total={percentagePixelsDrawing}%\n" +
            $"same {numSamePixelsTextures} => {percentageSame}\n" +
            $"extra {numExtraPixels} => {percentageExtra}\n" +
            $"{othersPattern} others pixels modified to black");*/

        float score = numSamePixelsTextures / numBlackPixelsTDrawing;
        results[0] = score;
        results[1] = percentageExtra;
        return results;
    }

    public void SetIsGesturing(bool b)
    {
        gesturing = b;
    }

    public bool GetIsGesturing()
    {
        return gesturing;
    }

    public void SetTextWidht(int w)
    {
        widthText = w;
    }
}