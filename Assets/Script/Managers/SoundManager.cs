using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// SoundManager is responsible to the volume control of any sfx. Only button clicking sfx for this project
public class SoundManager : MonoBehaviour {

    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }
    
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    

    private float volume = 1f;
    private void Awake() {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start() {
        
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f) {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumeMultiplier);
    }
    
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    public void PlayCommonButtonClickedSound()
    {
        PlaySound(audioClipRefsSO.ButtonClicked, Vector2.zero);
    }

    public void ChangeVolume() {
        volume += .1f;
        if (volume > 1f) {
            volume = 0f;
        }
        
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        
        // Manual Save to prevent failing to save due to crashing
        PlayerPrefs.Save();
    }
    
    public void ChangeVolume(float volume) {
        this.volume = volume;
        
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        
        // Manual Save to prevent failing to save due to crashing
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return volume;
    }
}