using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices; // Ensure this using directive is at the top


public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button GameLaunchButton;
    [SerializeField] private Button GameSettingsButton;
    [SerializeField] private Button ReturnButton;

    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SoundVolumeSlider;
    [SerializeField] private Slider DifficultyLevelSlider;
    
    [SerializeField] private GameObject GameLaunchUI;
    [SerializeField] private GameObject GameSettingsUI;
    
    // [SerializeField] private Button quitButton;
    
    // Assuming "myDynamicLibrary.dll" exports a function named "send_controllight"
    // that takes two parameters: a string (for IP) and an int (for port).
    [DllImport("myDynamicLibrary", CallingConvention = CallingConvention.Cdecl)]
    private static extern void send_controllight([MarshalAs(UnmanagedType.LPWStr)] string targetIP, int targetPort);    
    string targetIP = "169.254.161.94";
    int targetPort = 4628;
    
    private void Awake() {
        GameSettingsUI.SetActive(false);
        GameLaunchButton.onClick.AddListener(() => {
            send_controllight(targetIP, targetPort);
            Debug.Log("send_controllight(" + targetIP + ", " + targetPort + ")");
        });
        
        GameSettingsButton.onClick.AddListener(() =>
        {
            GameLaunchUI.SetActive(false);
            GameSettingsUI.SetActive(true);
            
        });
        
        ReturnButton.onClick.AddListener(() =>
        {
            GameSettingsUI.SetActive(false);
            GameLaunchUI.SetActive(true);
        });
        
        // MusicVolumeSlider.onValueChanged.AddListener(() =>
        // {
        //     MusicManager.Instance.ChangeVolume(MusicVolumeSlider.value);
        // });
        
        // Click quitButton
        // quitButton.onClick.AddListener(Application.Quit);
        
        // Time.timeScale = 1f;
    }
}
