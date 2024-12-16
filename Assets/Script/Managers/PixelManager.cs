using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Windows;
using ColorUtility = UnityEngine.ColorUtility;

public class PixelManager : MonoBehaviour
{
    public static PixelManager Instance { private set; get; }

    [SerializeField] private RawImage rawImage;
    
    // [SerializeField] private Toggle[] toggles;

    [SerializeField] private TextMeshProUGUI colorText;

    private string[][] pixels;
    private int[][][] array3D;
    private int[][][] edittingArray3D;
    private int[][] hardwareViewPixels;
    private int width;
    private int height;
    private int maxFrame;
    private int editingFrameIndex;
    private Color32[] colorPixels;

    private int selectedGridIndex;
    private string selectedGridColor;
    private int currentLoopIndex;

    private void Start()
    {
        Instance = this;
        maxFrame = 0;
        editingFrameIndex = 0;
        height = 0;
        width = 0;
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
        
        
    }

    public void ExtractPixel()
    {
        
        Texture2D texture2D = (Texture2D)rawImage.texture;
        width = rawImage.texture.width;
        height = rawImage.texture.height;

        colorPixels = texture2D.GetPixels32();
    
        // //Debug.Log(colorPixels.Length);

        ConvertStartingPixel();
        
        pixels = new string[height][];
        for (int i = 0; i < height; i++)
        {
            pixels[i] = new string[width];
        }
        
        Update2DPixelArray();

        Print2DPixelArray();

        // SavePixelsInfoToPref();

        // updateAllPixels();
    }
    
    public void ExtractPixelByFile()
    {
        array3D = FileManager.Instance.ReadArrayFile();
        // array3D = FileManager.Instance.ReadFrameFile(FileManager.Instance.OpenFileBrowserForTxt());
        // array3D = ConvertStringFramesToColorCodeFrames(FileManager.Instance.ReadTxtFile());
        // array3D = CombineRepeatedKeyFrames(array3D);
        // array3D = GetRefreshedKeyFrames(array3D, 3);

        //Debug.Log("Array3D.length: " + array3D[0].Length);
        edittingArray3D = array3D;

        maxFrame = array3D.Length;
        height = array3D[0].Length;
        width = array3D[0][0].Length;
        editingFrameIndex = 0;

        colorPixels = new Color32[height * width];

        SetColorPixels();   
        
        //Debug.Log(colorPixels.Length);
        
        pixels = new string[height][];
        for (int i = 0; i < height; i++)
        {
            pixels[i] = new string[width];
        }
        
        Update2DPixelArray();

        Print2DPixelArray();

        // SavePixelsInfoToPref();

        // updateAllPixels();
    }
    
    public void ExtractPixelByLocalVariable(int[][][] mapArray)
    {
        array3D = mapArray;

        //Debug.Log("Array3D.length: " + array3D[0].Length);
        edittingArray3D = array3D;

        maxFrame = array3D.Length;
        height = array3D[0].Length;
        width = array3D[0][0].Length;
        editingFrameIndex = 0;

        colorPixels = new Color32[height * width];

        SetColorPixels();   
        
        //Debug.Log(colorPixels.Length);
        
        pixels = new string[height][];
        for (int i = 0; i < height; i++)
        {
            pixels[i] = new string[width];
        }
        
        Update2DPixelArray();

        Print2DPixelArray();

        // SavePixelsInfoToPref();

        // updateAllPixels();
    }
    
    public float CalculateTotalDisplayTime(int[][][] resultArray)
    {
        int framesPerSecond = 12; // Maximum number of frames displayed per second
        int totalFrames = resultArray.Length; // Total number of frames in the array

        // Calculate the total display time
        float totalDisplayTime = (float)totalFrames / framesPerSecond;

        return totalDisplayTime;
    }
    
    public int[][][] GetRefreshedKeyFrames(int[][][] keyFrameArray, int keyFrameRefreshRate)
    {
        // Calculate the total number of frames in the result array
        int totalFrames = keyFrameArray.Length * keyFrameRefreshRate;
        int y = keyFrameArray[0].Length;
        int x = keyFrameArray[0][0].Length;

        // Initialize the result array
        int[][][] resultArray = new int[totalFrames][][];

        for (int i = 0; i < totalFrames; i++)
        {
            // Determine which key frame to use for this frame
            int keyFrameIndex = i / keyFrameRefreshRate;

            // Initialize the 2D array for this frame
            resultArray[i] = new int[y][];

            for (int j = 0; j < y; j++)
            {
                // Copy the key frame's 2D array to the result array
                resultArray[i][j] = new int[x];
                Array.Copy(keyFrameArray[keyFrameIndex][j], resultArray[i][j], x);
            }
        }

        return resultArray;
    }

    private int[][][] CombineRepeatedKeyFrames(int[][][] rawArray)
    {
        // Convert the 3D array to a list of 2D arrays
        List<int[][]> list = new List<int[][]>(rawArray);

        // Use HashSet to eliminate duplicates
        HashSet<int[][]> set = new HashSet<int[][]>(list, new ArrayEqualityComparer());

        // Convert back to 3D array
        int[][][] result = set.ToArray();

        return result;
    }

// Custom equality comparer for 2D arrays
    public class ArrayEqualityComparer : IEqualityComparer<int[][]>
    {
        public bool Equals(int[][] x, int[][] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }

            for (int i = 0; i < x.Length; i++)
            {
                if (!x[i].SequenceEqual(y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(int[][] obj)
        {
            int hash = 17;

            foreach (int[] array in obj)
            {
                foreach (int element in array)
                {
                    hash = hash * 31 + element.GetHashCode();
                }
            }

            return hash;
        }
    }

    private int[][][] ConvertStringFramesToColorCodeFrames(string[][][] stringFrames)
    {
        int[][][] intFrames = new int[stringFrames.Length][][];

        for (int d = 0; d < intFrames.Length; d++)
        {
            intFrames[d] = new int[stringFrames[0].Length][];
            for (int y = 0; y < intFrames[0].Length; y++)
            {
                intFrames[d][y] = new int[stringFrames[0][0].Length];
                for (int x = 0; x < intFrames[0][0].Length; x++)
                {
                    intFrames[d][y][x] = RGBStringTOColorCode(stringFrames[d][y][x]);
                }
            }
        }
        
        return intFrames;
    }

    public int RGBStringTOColorCode(string rgbString)
    {
        switch (rgbString)
        {
            case "0 255 0":
                return 0;
            case "255 0 0":
                return 1;
            case "0 0 255":
                return 2;
            default:
                return 4;
        }
    }

    private void SetColorPixels()
    {
        Color newColor;
        for (int y = 0; y < edittingArray3D[editingFrameIndex].Length; y++)
        {
            for (int x = 0; x < edittingArray3D[editingFrameIndex][y].Length; x++)
            {
                if (ColorUtility.TryParseHtmlString("#"+ColorCodeToHexCode(edittingArray3D[editingFrameIndex][y][x]), out newColor))
                {
                    //Debug.Log("Converting index at: " + (x + y * width));
                    Color32 newColor32 = new Color32((byte)(newColor.r * 255), (byte)(newColor.g * 255),
                        (byte)(newColor.b * 255), 255);
                    colorPixels[x + y * width] = newColor32;
                }
            }
        }
    }

    private void ConvertStartingPixel()
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
    //     //Debug.Log(height);
    //     //Debug.Log(width);
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
    
    public void UpdateAllPixelsDynamic(Toggle[] dynamicToggles)
    {
        //Debug.Log(height);
        //Debug.Log(width);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 newColor32 = new Color32(colorPixels[x + width * y].r, colorPixels[x + width * y].g,
                    colorPixels[x + width * y].b, 255);
                if (HexCodeToColorName(ColorUtility.ToHtmlStringRGBA(newColor32)).Equals("EMPTY"))
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

    private static int HexCodeToColorCode(string colorhexCode)
    {
        switch (colorhexCode)
        {
            case "13FF03FF":
                return 0;
            case "FF0000FF":
                return 1;
            case "450DFFFF":
                return 2;
            default:
                return 4;
            
        }
    }
    
    private static string HexCodeToColorName(string colorhexCode)
    {
        switch (colorhexCode)
        {
            case "13FF03FF":
                return "GREEN";
            case "FF0000FF":
                return "RED";
            case "450DFFFF":
                return "BLUE";
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
            case "RED":
                return "FF0000FF";
            case "BLUE":
                return "450DFFFF";
            case "EMPTY":
                return "7F7F7FFF";
            
        }
    }
    
    private string ColorCodeToHexCode(int colorCode)
    {
        switch (colorCode)
        {
            case 0:
                return "13FF03FF";
            case 1:
                return "FF0000FF";
            case 2:
                return "450DFFFF";
            default:
            case 4:
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

                colorText.text = HexCodeToColorName(selectedGridColor);           
            }
        });
    }

    public void SetColorOfOneGrid(string colorName, Toggle[] toggles)
    {
        colorText.text = colorName;
        
        Color newColor;
        Print2DPixelArray();
        if (ColorUtility.TryParseHtmlString("#"+ColorNameToHexCode(colorName), out newColor))
        {
            //Debug.Log("Altering..");
            Color32 newColor32 = new Color32((byte)(newColor.r * 255), (byte)(newColor.g * 255),
                (byte)(newColor.b * 255), 255);
            colorPixels[selectedGridIndex] = newColor32;
            toggles[selectedGridIndex].GetComponentInChildren<Image>().color = colorPixels[selectedGridIndex];
            Update2DPixelArray();
        }
        Print2DPixelArray();
    }

    private void Print2DPixelArray()
    {
        // for (int y = 0; y < height; y++)
        // {
        //     //Debug.Log(y + ". row: " );
        //     for (int x = 0; x < width; x++)
        //     {
        //         //Debug.Log(pixels[y,x]);
        //     }
        // }
    }

    private void Update2DPixelArray()
    {
        pixels = new string[height][];

        for (int y = 0; y < height; y++)
        {
            pixels[y] = new string[width];
            for (int x = 0; x < width; x++)
            {
                pixels[y][x] = ColorUtility.ToHtmlStringRGBA(colorPixels[x + width * y]);
                if (HexCodeToColorName(pixels[y][x]).Equals("EMPTY"))
                {
                    pixels[y][x] = ColorUtility.ToHtmlStringRGBA(Color.grey);
                }
            }
        }

        UpdateHardwareViewPixels();
    }
    
    private void Update2DPixelArrayByFile()
    {
        pixels = new string[height][];

        for (int y = 0; y < height; y++)
        {
            pixels[y] = new string[width];
            for (int x = 0; x < width; x++)
            {
                pixels[y][x] = ColorCodeToHexCode(array3D[editingFrameIndex][height][width]);
                if (HexCodeToColorName(pixels[y][x]).Equals("EMPTY"))
                {
                    pixels[y][x] = ColorUtility.ToHtmlStringRGBA(Color.grey);
                }
            }
        }

        UpdateHardwareViewPixels();
    }


    private void UpdateHardwareViewPixels()
    {
        hardwareViewPixels = new int[height][];

        for (int y = 0; y < height; y++)
        {
            hardwareViewPixels[y] = new int[width];
            for (int x = 0; x < width; x++)
            {
                hardwareViewPixels[y][x] = HexCodeToColorCode(pixels[y][x]);
            }
        }

        edittingArray3D[editingFrameIndex] = hardwareViewPixels;
    }


    public int GetWidth()
    {
        return width;
    }
    
    public int GetHeight()
    {
        return height;
    }

    public int[][] GetHardwareViewPixel()
    {
        return hardwareViewPixels;
    }

    public void SaveEditedArray()
    {
        array3D = edittingArray3D;
        FileManager.Instance.OutputArrayAsFile(array3D);
        SetColorPixels();
        
        Update2DPixelArray();
        //Debug.Log("Saved");
    }
    
    public void ResetEditedArray()
    {
        edittingArray3D = array3D;
        SetColorPixels();
        Update2DPixelArray();
    }

    public int GetEditingFrameIndex()
    {
        return editingFrameIndex;
    }
    
    public int GetMaxFrame()
    {
        return maxFrame;
    }

    public void MoveToPreviousFrame()
    {
        if (editingFrameIndex > 0)
        {
            editingFrameIndex -= 1;
            SetColorPixels();
            Update2DPixelArray();
            edittingArray3D[editingFrameIndex] = hardwareViewPixels;
        }
    }

    public void MoveToNextFrame()
    {
        if (editingFrameIndex < maxFrame - 1)
        {
            editingFrameIndex += 1;
            SetColorPixels();
            Update2DPixelArray();
            edittingArray3D[editingFrameIndex] = hardwareViewPixels;
        }
    }
    
    public void MoveToTheFrame(int theFrame)
    {
        if (editingFrameIndex < maxFrame - 1 || editingFrameIndex > 0)
        {
            editingFrameIndex = theFrame;
            SetColorPixels();
            Update2DPixelArray();
            edittingArray3D[editingFrameIndex] = hardwareViewPixels;
        }
    }

    public void TestReadTxt()
    {
        int[][][] testingArray = FileManager.Instance.ReadFrameFile(FileManager.Instance.OpenFileBrowserForTxt());
        //Debug.Log(testingArray.Length);
        for (int d = 0; d < testingArray.Length; d++)
        {
            //Debug.Log("Frame " + d + ": ");
            for (int y = 0; y < testingArray[0].Length; y++)
            {
                for (int x = 0; x < testingArray[0][0].Length; x++)
                {
                    //Debug.Log(testingArray[d][y][x]);
                }
            }
        }
    }
}
