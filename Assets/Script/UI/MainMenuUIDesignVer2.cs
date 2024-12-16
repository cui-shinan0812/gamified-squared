using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIDesignVer2 : MonoBehaviour
{

    // name of each item of player preferences
    protected const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    protected const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    protected const string PLAYER_PREFS_DIFFICULTY_LEVEL = "DifficultyLevel";
    protected const string PLAYER_PREFS_BACKGROUND_DISPLAY_MODE = "BackgroundDisplayMode";
    
    [SerializeField] private GameObject SettingsUI;
    [SerializeField] private GameObject MainUIBackgroundImage;
    [SerializeField] private GameObject VideoPlayer;

    [SerializeField] private Button SettingsButton;
    
    [SerializeField] private RawImage backgroundRawImage;
    
    
    private void Awake()
    {
        
        SettingsUI.SetActive(false);
        
        SettingsButton.onClick.AddListener(() =>
        {
            SettingsUI.SetActive(true);
            gameObject.SetActive(false);
        });
        
        // if (PlayerPrefs.GetInt(PLAYER_PREFS_BACKGROUND_DISPLAY_MODE, 0) == 0)
        // {
        //     MainUIBackgroundImage.SetActive(true);
        //     VideoPlayer.SetActive(false);
        // }
        // else
        // {
        //     MainUIBackgroundImage.SetActive(false);
        //     VideoPlayer.SetActive(true);
        // }
        
        // FileManager.Instance.setBackgroundImagePath(
        //     PlayerPrefs.GetString(FileManager.PLAYER_PREFS_BACKGROUND_IMAGE_PATH, "D:\\works\\Sportopia\\game\\unity\\Assets\\Images\\background\\Blue Grid Wallpaper.jpg"));
        // FileManager.Instance.setBackgroundVideoPath(
        //     PlayerPrefs.GetString(FileManager.PLAYER_PREFS_BACKGROUND_VIDEO_PATH, "D:\\works\\Sportopia\\game\\download\\triAnimate.mp4"));
        // FileManager.Instance.setLogoImagePath(
        //     PlayerPrefs.GetString(FileManager.PLAYER_PREFS_LOGO_IMAGE_PATH, "D:\\works\\Sportopia\\game\\unity\\Assets\\Images\\Icons\\Daco_1917848.png"));
    }

    public GameObject GetMainUIBackgroundImage()
    {
        return MainUIBackgroundImage;
    }

    public GameObject GetVideoPlayer()
    {
        return VideoPlayer;
    }

    public string GetPLAYER_PREFS_MUSIC_VOLUME()
    {
        return PLAYER_PREFS_MUSIC_VOLUME;
    }
    
    public string GetPLAYER_PREFS_SOUND_EFFECTS_VOLUME()
    {
        return PLAYER_PREFS_SOUND_EFFECTS_VOLUME;
    }
    
    public string GetPLAYER_PREFS_DIFFICULTY_LEVEL()
    {
        return PLAYER_PREFS_DIFFICULTY_LEVEL;
    }
    
    public string GetPLAYER_PREFS_BACKGROUND_DISPLAY_MODE()
    {
        return PLAYER_PREFS_BACKGROUND_DISPLAY_MODE;
    }
}
