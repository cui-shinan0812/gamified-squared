using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting; // Ensure this using directive is at the top

public class GameLaunchUI : MainMenuUI
{
    
    [SerializeField] private Button GameLaunchButton;
    [SerializeField] private Button GameSettingsButton;
    
    // [SerializeField] private Button quitButton;
    
    // Assuming "myDynamicLibrary.dll" exports a function named "send_controllight"
    // that takes two parameters: a string (for IP) and an int (for port).
    [DllImport("myDynamicLibrary", CallingConvention = CallingConvention.Cdecl)]
    private static extern void send_controllight([MarshalAs(UnmanagedType.LPWStr)] string targetIP, int targetPort);    
    string targetIP = "169.254.161.94";
    int targetPort = 4628;
    
    public void Clickonbutton()
    {
        send_controllight(targetIP, targetPort);
    }
    
    private void Awake() {
        
        GameLaunchButton.onClick.AddListener(() => {
            send_controllight(targetIP, targetPort);
            Debug.Log("send_controllight(" + targetIP + ", " + targetPort + ")");
        });
        
        GameSettingsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayCommonButtonClickedSound();
            gameObject.SetActive(false);
            GameSettingsUI.SetActive(true);

        });
        
        // Click quitButton
        // quitButton.onClick.AddListener(Application.Quit);
        
        // Time.timeScale = 1f;
    }
}
