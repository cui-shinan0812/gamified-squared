using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class ScenesManager : MonoBehaviour
{
    public void LoadMode(){
        SceneManager.LoadScene("Mode");
    }
    public void LoadRule(){
        SceneManager.LoadScene("Rule");
    }
    public void LoadMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadMaps(){
        SceneManager.LoadScene("Maps");
    }
    public void LoadFloor(){
        SceneManager.LoadScene("Floor");
    }
    public void LoadSettings(){
        SceneManager.LoadScene("Settings");
    }
    public void LoadAbout(){
        SceneManager.LoadScene("About");
    }
}
