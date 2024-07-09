using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
 
public class VideoLoader : MonoBehaviour
{
    public static VideoLoader Instance { get; private set; }

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoUrl = "";
     
    void Awake()
    {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance!");
        }

        Instance = this;
        videoPlayer.url = videoUrl;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack (0, true);
        videoPlayer.Prepare ();
    }

    public void setVideoURL(string url)
    {
        videoUrl = url;
        UpdateVideoUrl();
    }

    private void UpdateVideoUrl()
    {
        videoPlayer.url = videoUrl;
    }
}