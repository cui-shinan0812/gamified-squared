using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    
    [SerializeField] protected GameObject GameLaunchUI;
    [SerializeField] protected GameObject GameSettingsUI;
    
    [SerializeField] protected GameObject MainUIBackgroundImage;
    [SerializeField] protected GameObject VideoPlayer;
    
    
    private void Awake() {
        
        GameLaunchUI.SetActive(true);
        GameSettingsUI.SetActive(false);
        
        MainUIBackgroundImage.SetActive(true);
        VideoPlayer.SetActive(false);

    }
}
