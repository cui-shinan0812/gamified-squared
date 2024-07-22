using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class PixelManager : MonoBehaviour
{
    public static PixelManager Instance { private set; get; }

    [SerializeField] private RawImage rawImage;
    
    // [SerializeField] private Toggle[] toggles;

    [SerializeField] private TextMeshProUGUI colorText;

    private string[,] pixels;
    private int width;
    private int height;
    private Color32[] colorPixels;

    private int selectedGridIndex;
    private string selectedGridColor;
    private int currentLoopIndex;

    private void Start()
    {
        Instance = this;
        // GameplayManager.Instance.OnStateChanged += GameplayManager_OnStateChanged;
    }

    // private void GameplayManager_OnStateChanged(object sender, EventArgs e)
    // {
    //     if (GameplayManager.Instance.IsGamePlay())
    //     {
    //         
    //     }
    // }

    private void Update()
    {
        if (GameplayManager.Instance.IsGamePlay())
        {
            // VirtualReceiver.Instance.displayFrame(pixels);
        }
    }

    public void extractPixel()
    {
        
        Texture2D texture2D = (Texture2D)rawImage.texture;
        width = rawImage.texture.width;
        height = rawImage.texture.height;

        colorPixels = texture2D.GetPixels32();
    
        Debug.Log(colorPixels.Length);

        convertStartingPixel();
        
        pixels = new string[height,width];    
        
        update2DPixelArray();

        print2DPixelArray();

        // updateAllPixels();
    }

    private void convertStartingPixel()
    {
        Color32[] newPixelArray = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                // Convert the index to start from the top left corner
                int newIndex = (height - y - 1) * width + x;
                newPixelArray[newIndex] = colorPixels[index];
            }
        }
        
        colorPixels = newPixelArray;
    }

    // public void updateAllPixels()
    // {
    //     Debug.Log(height);
    //     Debug.Log(width);
    //     for (int y = 0; y < height; y++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             Color32 newColor32 = new Color32(colorPixels[x + width * y].r, colorPixels[x + width * y].g,
    //                 colorPixels[x + width * y].b, 255);
    //             if (hexCodeToColorName(ColorUtility.ToHtmlStringRGBA(newColor32)).Equals("EMPTY"))
    //             {
    //                 toggles[x + width * y].GetComponentInChildren<Image>().color = Color.grey;
    //             }
    //             else
    //             {
    //                 toggles[x + width * y].GetComponentInChildren<Image>().color = newColor32;
    //             }
    //         }
    //     }
    // }
    
    public void updateAllPixelsDynamic(Toggle[] dynamicToggles)
    {
        Debug.Log(height);
        Debug.Log(width);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 newColor32 = new Color32(colorPixels[x + width * y].r, colorPixels[x + width * y].g,
                    colorPixels[x + width * y].b, 255);
                if (hexCodeToColorName(ColorUtility.ToHtmlStringRGBA(newColor32)).Equals("EMPTY"))
                {
                    dynamicToggles[x + width * y].GetComponentInChildren<Image>().color = Color.grey;
                }
                else
                {
                    dynamicToggles[x + width * y].GetComponentInChildren<Image>().color = newColor32;
                }
            }
        }
    }

    private int hexCodeToColorCode(string colorhexCode)
    {
        switch (colorhexCode)
        {
            case "13FF03FF":
                return 1;
            case "450DFFFF":
                return 2;
            case "FF0000FF":
                return 3;
            default:
                return 0;
            
        }
    }
    
    private string hexCodeToColorName(string colorhexCode)
    {
        switch (colorhexCode)
        {
            case "13FF03FF":
                return "GREEN";
            case "450DFFFF":
                return "BLUE";
            case "FF0000FF":
                return "RED";
            default:
                return "EMPTY";
            
        } 
    }
    
    private string ColorNameToHexCode(string colorName)
    {
        switch (colorName)
        {
            default:
            case "GREEN":
                return "13FF03FF";
            case "BLUE":
                return "450DFFFF";
            case "RED":
                return "FF0000FF";
            case "EMPTY":
                return "7F7F7FFF";
            
        }
    }
    
    public void AddButtonListener(Toggle toggle, int index)
    {
        toggle.onValueChanged.AddListener((flag) =>
        {
            if (flag)
            {
                selectedGridIndex = index;
                selectedGridColor = ColorUtility.ToHtmlStringRGBA(toggle.GetComponentInChildren<Image>().color);

                colorText.text = hexCodeToColorName(selectedGridColor);           
            }
        });
    }

    public void SetColorOfOneGrid(string colorName, Toggle[] toggles)
    {
        colorText.text = colorName;
        
        Color newColor;
        print2DPixelArray();
        if (ColorUtility.TryParseHtmlString("#"+ColorNameToHexCode(colorName), out newColor))
        {
            Debug.Log("Altering..");
            Color32 newColor32 = new Color32((byte)(newColor.r * 255), (byte)(newColor.g * 255),
                (byte)(newColor.b * 255), 255);
            colorPixels[selectedGridIndex] = newColor32;
            toggles[selectedGridIndex].GetComponentInChildren<Image>().color = colorPixels[selectedGridIndex];
            update2DPixelArray();
        }
        print2DPixelArray();
    }

    private void print2DPixelArray()
    {
        for (int y = 0; y < height; y++)
        {
            Debug.Log(y + ". row: " );
            for (int x = 0; x < width; x++)
            {
                Debug.Log(pixels[y,x]);
            }
        }
    }

    private void update2DPixelArray()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[y,x] = ColorUtility.ToHtmlStringRGBA(colorPixels[x + width * y]);
                if (hexCodeToColorName(pixels[y,x]).Equals("EMPTY"))
                {
                    pixels[y, x] = ColorUtility.ToHtmlStringRGBA(Color.grey);
                }
            }
        }
    }

    public int getWidth()
    {
        return width;
    }
    
    public int getHeight()
    {
        return height;
    }
}
