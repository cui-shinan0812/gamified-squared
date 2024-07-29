using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting; // Ensure this using directive is at the top

public class GameLaunchUI : MainMenuUI
{
    public static GameLaunchUI Instance;
    
    [SerializeField] private Button GameLaunchButton;
    [SerializeField] private Button GameSettingsButton;
    [SerializeField] private Button ConfigButton;
    [SerializeField] private Button ReadFileButton;
    [SerializeField] private Button WriteFileButton;

    // [SerializeField] private Button quitButton;
    
    // Assuming "myDynamicLibrary.dll" exports a function named "send_controllight"
    // that takes two parameters: a string (for IP) and an int (for port).
    [DllImport("myDynamicLibrary", CallingConvention = CallingConvention.Cdecl)]
    private static extern void send_controllight([MarshalAs(UnmanagedType.LPWStr)] string targetIP, int targetPort);    
    string targetIP = "169.254.161.94";
    int targetPort = 4628;

    private int[] configInfo = new int[4];
        
    public void Clickonbutton()
    {
        send_controllight(targetIP, targetPort);
    }
    
    private void Awake()
    {
        Instance = this;
        
        GameLaunchButton.onClick.AddListener(() => {
            send_controllight(targetIP, targetPort);
            Debug.Log("send_controllight(" + targetIP + ", " + targetPort + ")");
            
            FileManager.Instance.readTextFile(PlayerPrefs.GetString(FileManager.PLAYER_PREFS_CONFIG_PATH));

            Loader.Load(Loader.Scene.GameplayScene);
        });
        
        GameSettingsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayCommonButtonClickedSound();
            gameObject.SetActive(false);
            GameSettingsUI.SetActive(true);

        });
        
        ConfigButton.onClick.AddListener(() =>
        {
            FileManager.Instance.OpenFileBrowserForConfig();
        });
        
        ReadFileButton.onClick.AddListener(() =>
        {
            int[][][] my3DArray = FileManager.Instance.ReadArrayFile();
            
            Debug.Log(my3DArray.Length);
            for (int d = 0; d < my3DArray.Length; d++)
            {
                for (int y = 0; y < my3DArray[0].Length; y++)
                {
                    for (int x = 0; x < my3DArray[0][0].Length; x++)
                    {
                        Debug.Log("[my3DArray]" + my3DArray[d][y][x] + " ");
                    }
                }
            }
        });
        
        WriteFileButton.onClick.AddListener(() =>
        {
            FileManager.Instance.OutputArrayAsFile();
        });
        
        // Click quitButton
        // quitButton.onClick.AddListener(Application.Quit);
        
        // Time.timeScale = 1f;
    }

    public void SetConfigInfo(int value, int index)
    {
        configInfo[index] = value;
        Debug.Log("Config Set " + "index " + index + ": " + value);
    }
}
