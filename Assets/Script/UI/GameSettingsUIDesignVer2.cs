using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Christina.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameSettingsUIDesignVer2 : MonoBehaviour
{
    
    [Header("Buttons")]
    [SerializeField] private Button ReturnButton;
    // [SerializeField] private Button ChangeBackgroundImageButton;
    // [SerializeField] private Button ChangeBackgroundVideoButton;
    // [SerializeField] private Button ChangeLogoButton;
    // [SerializeField] private Button GoToEditModeButton;


    [Header("Sliders")]
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SoundVolumeSlider;
    // [SerializeField] private Slider DifficultyLevelSlider;

    [Header("Switches")]
    // [SerializeField] private Slider BackgroundDisplayModeSwitch;
    
    [Header("Text tags")]
    [SerializeField] private TextMeshProUGUI MusicVolumeValueText;
    [SerializeField] private TextMeshProUGUI SoundVolumeValueText;
    // [SerializeField] private TextMeshProUGUI DifficultyLevelValueText;
    // [SerializeField] private TextMeshProUGUI ImageTagText;
    // [SerializeField] private TextMeshProUGUI VideoTagText;
    
    [Header("Scripts")]
    // [SerializeField] private ToggleSwitch BackgroundDisplayModeToggleSwitch;
    
    [Header("UIs")]
    [SerializeField] private GameObject MainMenuUI;

    private MainMenuUIDesignVer2 mainMenuUIScript;
    
    private void Awake()
    {
        mainMenuUIScript = MainMenuUI.GetComponent<MainMenuUIDesignVer2>();
        // ChangeBackgroundImageButton.gameObject.SetActive(true);
        // ChangeBackgroundVideoButton.gameObject.SetActive(false);

        // BackgroundDisplayModeSwitch.value = PlayerPrefs.GetInt(mainMenuUIScript.GetPLAYER_PREFS_BACKGROUND_DISPLAY_MODE(), 0);
        // BackgroundDisplayModeToggleSwitch.setSliderValue();
        
        ReturnButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayCommonButtonClickedSound();
            gameObject.SetActive(false);
            MainMenuUI.SetActive(true);
        });
        
        // GoToEditModeButton.onClick.AddListener(() =>
        // {
        //     Loader.Load(Loader.Scene.EditSceneDynamicGridSize);
        // });
        
        // range of volume in PlayerPrefs is float number between 0 ~ 1, when display to user, *100 to make it an integer
        MusicVolumeSlider.value = PlayerPrefs.GetFloat(mainMenuUIScript.GetPLAYER_PREFS_MUSIC_VOLUME()) * 100f;
        MusicVolumeValueText.text = PlayerPrefs.GetFloat(mainMenuUIScript.GetPLAYER_PREFS_MUSIC_VOLUME()) * 100f + "";
        MusicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            // Since max value of the slide is 100, the value is scaled down to match range of the volume setting (0f - 1f)
            MusicVolumeValueText.text = value + "";
            MusicManager.Instance.ChangeVolume(value / 100f);
            
        });
        
        SoundVolumeSlider.value = PlayerPrefs.GetFloat(mainMenuUIScript.GetPLAYER_PREFS_SOUND_EFFECTS_VOLUME()) * 100f;
        SoundVolumeValueText.text = PlayerPrefs.GetFloat(mainMenuUIScript.GetPLAYER_PREFS_SOUND_EFFECTS_VOLUME()) * 100f + "";
        SoundVolumeSlider.onValueChanged.AddListener((value) =>
        {
            // Since max value of the slide is 100, the value is scaled down to match range of the volume setting (0f - 1f)
            SoundVolumeValueText.text = value + "";
            SoundManager.Instance.ChangeVolume(value / 100f);
            
        });
        
        // DifficultyLevelSlider.value = PlayerPrefs.GetInt(PLAYER_PREFS_DIFFICULTY_LEVEL);
        string difficultyText = getDifficultyText(PlayerPrefs.GetInt(mainMenuUIScript.GetPLAYER_PREFS_DIFFICULTY_LEVEL()));
        // DifficultyLevelValueText.text = difficultyText;
        // DifficultyLevelSlider.onValueChanged.AddListener((value) =>
        // {
        //     difficultyText = getDifficultyText((int)value);
        //     DifficultyLevelValueText.text = difficultyText;
        //     PlayerPrefs.SetInt(PLAYER_PREFS_DIFFICULTY_LEVEL, (int)value);
        //     PlayerPrefs.Save();
        //     
        // });
        
        // ImageTagText.color = Color.white;
        // VideoTagText.color = Color.gray;
        
        // BackgroundDisplayModeSwitch.onValueChanged.AddListener((flag) =>
        // {
        //     int f = (int)flag;
        //     if (f == 0)
        //     {
        //         // ImageTagText.color = Color.white;
        //         // VideoTagText.color = Color.gray;
        //         mainMenuUIScript.GetMainUIBackgroundImage().SetActive(true);
        //         mainMenuUIScript.GetVideoPlayer().SetActive(false);
        //         // ChangeBackgroundImageButton.gameObject.SetActive(true);
        //         // ChangeBackgroundVideoButton.gameObject.SetActive(false);
        //     }
        //     else
        //     {
        //         // ImageTagText.color = Color.gray;
        //         // VideoTagText.color = Color.white;
        //         mainMenuUIScript.GetMainUIBackgroundImage().SetActive(false);
        //         mainMenuUIScript.GetVideoPlayer().SetActive(true);
        //         // ChangeBackgroundImageButton.gameObject.SetActive(false);
        //         // ChangeBackgroundVideoButton.gameObject.SetActive(true);
        //     }
        //
        //     PlayerPrefs.SetInt(mainMenuUIScript.GetPLAYER_PREFS_BACKGROUND_DISPLAY_MODE(), f);
        //     PlayerPrefs.Save();
        // });

        // Click quitButton
        // quitButton.onClick.AddListener(Application.Quit);

        // Time.timeScale = 1f;
    }

    private string getDifficultyText(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            default:
            case 0:
                return "EASY";
            case 1:
                return "NORMAL";
            case 2:
                return "HARD";
            case 3:
                return "VERY HARD";
        }
    }
}
