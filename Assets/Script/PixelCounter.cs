using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.ExternalScripts;

public class PixelCounter : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    private void Start()
    {
        
    }

    public void extractPixel()
    {
        Color colorOnePix;
        Color32[] colorPixels;

        int width = rawImage.texture.width;
        int height = rawImage.texture.height;

        Texture2D texture2D = (Texture2D)rawImage.texture;

        colorOnePix = texture2D.GetPixel(50,53);
        colorPixels = texture2D.GetPixels32();

        string hexcode = ColorUtility.ToHtmlStringRGBA(colorOnePix);
        Debug.Log(colorPixels.Length);

        // for (int i = 0; i < colorPixels.Length; i++)
        // {
        //     Debug.Log(ColorUtility.ToHtmlStringRGBA(colorPixels[i]));
        // }

        string[,] pixels = new string[height,width];    
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[y,x] = ColorUtility.ToHtmlStringRGBA(colorPixels[x + width * y]);
            }
        }

        for (int y = 0; y < height; y++)
        {
            Debug.Log(y + ". row: " );
            for (int x = 0; x < width; x++)
            {
                Debug.Log(pixels[y,x]);
            }
        }
        // Debug.Log(hexcode);
        // Debug.Log(ColorUtility.ToHtmlStringRGBA(colorPixels[50 + width * 50]));
    }   
}
