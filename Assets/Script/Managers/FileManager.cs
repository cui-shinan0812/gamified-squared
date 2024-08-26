using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;


// FileManager is responsible to any function that require file access like opening file browser
public class FileManager : MonoBehaviour
{
    // const variable to store the player preference name
    public const string PLAYER_PREFS_BACKGROUND_IMAGE_PATH = "BackgroundImagePath";
    public const string PLAYER_PREFS_BACKGROUND_VIDEO_PATH = "BackgroundVideoPath";
    public const string PLAYER_PREFS_LOGO_IMAGE_PATH = "LogoImagePath";
    public const string PLAYER_PREFS_CONFIG_PATH = "ConfigPath";
    
    [SerializeField] private RawImage BackgroundRawImage;
    [SerializeField] private RawImage LogoRawImage;
    
    public static FileManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OpenFileBrowserForBackgroundImage()
    {
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            //Debug.Log(path);
            StartCoroutine(LoadImage(path, BackgroundRawImage));
            PlayerPrefs.SetString(PLAYER_PREFS_BACKGROUND_IMAGE_PATH, path);
            PlayerPrefs.Save();
        });
    }
    
    public void OpenFileBrowserForLogoImage()
    {
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.png) | *.png";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            //Debug.Log(path);
            StartCoroutine(LoadImage(path, LogoRawImage));
            PlayerPrefs.SetString(PLAYER_PREFS_LOGO_IMAGE_PATH, path);
            PlayerPrefs.Save();
        });
    }
    
    public void OpenFileBrowserForBackgroundVideos()
    {
        var bp = new BrowserProperties();
        bp.filter = "Video files (*.mp4) | *.mp4;";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Debug.Log(path);
            setBackgroundVideoPath(path);
            PlayerPrefs.SetString(PLAYER_PREFS_BACKGROUND_VIDEO_PATH, path);
            PlayerPrefs.Save();
        });
    }
    
    public void OpenFileBrowserForConfig()
    {
        var bp = new BrowserProperties();
        bp.filter = "text files (*.txt) | *.txt;";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Debug.Log(path);
            PlayerPrefs.SetString(PLAYER_PREFS_CONFIG_PATH, path);
            PlayerPrefs.Save();
        });
    }
    
    public string OpenFileBrowserForTxt()
    {
        var bp = new BrowserProperties();
        bp.filter = "text files (*.txt) | *.txt;";
        bp.filterIndex = 0;

        string txtPath = "";
        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Debug.Log(path);
            txtPath = path;
        });
        
        //Debug.Log(txtPath);

        return txtPath;
    }

    public void setBackgroundImagePath(string path)
    {
        StartCoroutine(LoadImage(path, BackgroundRawImage));
    }
    
    public void setLogoImagePath(string path)
    {
        StartCoroutine(LoadImage(path, LogoRawImage));
    }

    public void setBackgroundVideoPath(string path)
    {
        VideoLoader.Instance.setVideoURL(path);
    }
    
    public void readTextFile(string file_path)
    {
        StreamReader inp_stm = new StreamReader(file_path);

        int counter = 0;
        while(!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine( );
            // Do Something with the input.
            int value;

            if (Int32.TryParse(inp_ln, out value))
            {
                GameLaunchUI.Instance.SetConfigInfo(value, counter);
            }

            counter++;

        }

        inp_stm.Close( );  
    }

    public void OutputArrayAsFile()
    {
        int[][][] myJagged3DArray = new int[4][][];

        for (int d = 0; d < 4; d++)
        {
            myJagged3DArray[d] = new int[2][];
            for (int y = 0; y < 2; y++)
            {
                myJagged3DArray[d][y] = new int[2];
                for (int x = 0; x < 2; x++)
                {
                    // Assign random value in the range [0, 2]
                    myJagged3DArray[d][y][x] = UnityEngine.Random.Range(0, 3);
                }
            }
        }

        // Convert to a nested list
        List<List<List<int>>> nestedList = new List<List<List<int>>>();

        foreach (var layer in myJagged3DArray)
        {
            var innerList = new List<List<int>>();
            foreach (var row in layer)
            {
                innerList.Add(new List<int>(row));
            }
            nestedList.Add(innerList);
        }

        // Serialize the nested list to JSON
        string json = JsonConvert.SerializeObject(nestedList);

        // Open a file browser for the user to choose where to save the file
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
        saveFileDialog.FilterIndex = 1;
        saveFileDialog.RestoreDirectory = true;

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            // Save the JSON to the chosen file
            File.WriteAllText(saveFileDialog.FileName, json);
        }
    }
    
    public void OutputArrayAsFile(int[][][] myJagged3DArray)
    {
        if (myJagged3DArray.Length > 0)
        {
            // Convert to a nested list
            List<List<List<int>>> nestedList = new List<List<List<int>>>();
        
            foreach (var layer in myJagged3DArray)
            {
                var innerList = new List<List<int>>();
                foreach (var row in layer)
                {
                    innerList.Add(new List<int>(row));
                }
                nestedList.Add(innerList);
            }
        
            //Debug.Log("nestedList.Count: " + nestedList.Count);
        
            // Serialize the nested list to JSON
            string json = JsonConvert.SerializeObject(nestedList);

            // Open a file browser for the user to choose where to save the file
            string path = UnityEditor.EditorUtility.SaveFilePanel(
                "Save JSON file",
                "",
                "MyJSON.json",
                "json");
            if (!string.IsNullOrEmpty(path))
            {
                // Save the JSON to the chosen file
                File.WriteAllText(path, json);
            }
        }
        else
        {
            //Debug.LogError("Empty 3D Array");
        }
    }
    
    public int[][][] ReadArrayFile()
    {
        int[][][] myJagged3DArray = new int[][][] { };

        // Open file browser
        // string path = EditorUtility.OpenFilePanel("Select JSON file", "", "json");
        string path = "Assets\\Resources\\testPathMemMap.json";
        if (path.Length != 0)
        {
            // Read the JSON file content
            string jsonContent = File.ReadAllText(path);

            // Deserialize the JSON into a nested list
            List<List<List<int>>> nestedList = JsonConvert.DeserializeObject<List<List<List<int>>>>(jsonContent);

            int depth = nestedList.Count;
            int ySize = nestedList[0].Count;
            int xSize = nestedList[0][0].Count;

            // Create the 3D array
            myJagged3DArray = new int[depth][][];
            for (int d = 0; d < depth; d++)
            {
                myJagged3DArray[d] = new int[ySize][];
                for (int y = 0; y < ySize; y++)
                {
                    myJagged3DArray[d][y] = new int[xSize];
                    for (int x = 0; x < xSize; x++)
                    {
                        myJagged3DArray[d][y][x] = nestedList[d][y][x];
                    }
                }
            }
        }

        return myJagged3DArray;
    }
    
    public int[][][] ReadFrameFile(string filePath)
    {
        List<int[][]> framesList = new List<int[][]>();
        int ySize = 0;
        int xSize = 0;

        string[] lines = File.ReadAllLines(filePath);
        bool readingFrame = false;
        List<int> currentFrameLine = new List<int>();

        foreach (string line in lines)
        {
            if (line.StartsWith("Frame shape"))
            {
                if (currentFrameLine.Count > 0)
                {
                    framesList.Add(ConvertListTo2DArray(currentFrameLine, xSize));
                    currentFrameLine.Clear();
                }

                MatchCollection matches = Regex.Matches(line, @"\d+");
                if (matches.Count == 3)
                {
                    ySize = int.Parse(matches[0].Value);
                    xSize = int.Parse(matches[1].Value); // Do not multiply by 3, we're now storing color codes
                }
            }
            else if (line.StartsWith(" [[["))
            {
                readingFrame = true;
            }
            else if (line.Contains("]]]"))
            {
                readingFrame = false;
                currentFrameLine.AddRange(ExtractColorCodes(line));
            }
            else if (readingFrame)
            {
                currentFrameLine.AddRange(ExtractColorCodes(line));
            }
        }

        // Add the last frame if it exists
        if (currentFrameLine.Count > 0)
        {
            framesList.Add(ConvertListTo2DArray(currentFrameLine, xSize));
        }

        return framesList.ToArray();
    }
    
    private int[] ExtractColorCodes(string line)
    {
        var colorCodes = new List<int>();
        var matches = Regex.Matches(line, @"\d+");

        for (int i = 0; i < matches.Count; i += 3)
        {
            int r = int.Parse(matches[i].Value);
            int g = int.Parse(matches[i + 1].Value);
            int b = int.Parse(matches[i + 2].Value);

            if (r == 255 && g == 0 && b == 0)
            {
                colorCodes.Add(1); // Red
            }
            else if (r == 0 && g == 255 && b == 0)
            {
                colorCodes.Add(0); // Green
            }
            else if (r == 0 && g == 0 && b == 255)
            {
                colorCodes.Add(2); // Blue
            }
            else
            {
                colorCodes.Add(-1); // Undefined color
            }
        }

        return colorCodes.ToArray();
    }
    
    public int[][] ConvertListTo2DArray(List<int> list, int xSize)
    {
        int ySize = list.Count / xSize;
        int[][] array2D = new int[ySize][];

        for (int i = 0; i < ySize; i++)
        {
            array2D[i] = new int[xSize];
            for (int j = 0; j < xSize; j++)
            {
                array2D[i][j] = list[i * xSize + j];
            }
        }

        return array2D;
    }

    public string[][][] ReadTxtFile()
    {
        string[][][] frames = File.ReadAllText("D:\\works\\Sportopia\\game\\unity\\Assets\\Resources\\output.txt")
            .Split(new[] { "Frame shape: (4, 4, 3)\r\nFrame values:\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(frameText => frameText
                .Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(rowText => rowText
                    .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(pixelText => pixelText
                        .Trim('[', ' ', ']')
                        .Replace("   ", " "))
                    .ToArray())
                .ToArray())
            .ToArray();

        // Print the first pixel of the first frame
        for (int x = 0; x < frames[0][0].Length; x++)
        {
            //Debug.Log("frame: " + string.Join(" ", frames[0][1][x]));
        }
        
        return frames;
    }
    
    IEnumerator LoadImage(string path, RawImage rawImage)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                //Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                rawImage.texture = uwrTexture;
            }
        }
    }
    
}
