using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    // name of each item of player preferences
    protected const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    protected const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    protected const string PLAYER_PREFS_DIFFICULTY_LEVEL = "DifficultyLevel";
    protected const string PLAYER_PREFS_BACKGROUND_DISPLAY_MODE = "BackgroundDisplayMode";

    
    [SerializeField] protected GameObject GameLaunchUI;
    [SerializeField] protected GameObject GameSettingsUI;
    
    [SerializeField] protected GameObject MainUIBackgroundImage;
    [SerializeField] protected GameObject VideoPlayer;

    [SerializeField] private RawImage backgroundrRawImage;
    
    
    private void Awake()
    {
        
        GameLaunchUI.SetActive(true);
        GameSettingsUI.SetActive(false);

        if (PlayerPrefs.GetInt(PLAYER_PREFS_BACKGROUND_DISPLAY_MODE, 0) == 0)
        {
            MainUIBackgroundImage.SetActive(true);
            VideoPlayer.SetActive(false);
        }
        else
        {
            MainUIBackgroundImage.SetActive(false);
            VideoPlayer.SetActive(true);
        }
        
        FileManager.Instance.setBackgroundImagePath(
            PlayerPrefs.GetString(FileManager.PLAYER_PREFS_BACKGROUND_IMAGE_PATH, "D:\\works\\Sportopia\\game\\unity\\Assets\\Images\\background\\Blue Grid Wallpaper.jpg"));
        FileManager.Instance.setBackgroundVideoPath(
            PlayerPrefs.GetString(FileManager.PLAYER_PREFS_BACKGROUND_VIDEO_PATH, "D:\\works\\Sportopia\\game\\download\\triAnimate.mp4"));
        FileManager.Instance.setLogoImagePath(
            PlayerPrefs.GetString(FileManager.PLAYER_PREFS_LOGO_IMAGE_PATH, "D:\\works\\Sportopia\\game\\unity\\Assets\\Images\\Icons\\Daco_1917848.png"));
    }
}
