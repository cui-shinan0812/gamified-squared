using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using UnityEngine.Video;

public class FileManager : MonoBehaviour
{
    private const string PLAYER_PREFS_BACKGROUND_IMAGE_PATH = "BackgroundImagePath";
    private const string PLAYER_PREFS_BACKGROUND_VIDEO_PATH = "BackgroundVideoPath";
    private const string PLAYER_PREFS_LOGO_IMAGE_PATH = "LogoImagePath";
    
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
            Debug.Log(path);
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
            Debug.Log(path);
            StartCoroutine(LoadImage(path, LogoRawImage));
            PlayerPrefs.SetString(PLAYER_PREFS_LOGO_IMAGE_PATH, path);
            PlayerPrefs.Save();
        });
    }
    
    public void OpenFileBrowserForBackgroundVideos()
    {
        var bp = new BrowserProperties();
        bp.filter = "Video files (*.mp4) | *.mp4; *.webm; *.mov; *.avi; *.wmv; *.flv";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            Debug.Log(path);
            setBackgroundVideoPath(path);
            PlayerPrefs.SetString(PLAYER_PREFS_BACKGROUND_VIDEO_PATH, path);
            PlayerPrefs.Save();
        });
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
    
    IEnumerator LoadImage(string path, RawImage rawImage)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                rawImage.texture = uwrTexture;
            }
        }
    }
}
