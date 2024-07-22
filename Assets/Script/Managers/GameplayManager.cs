using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { private set; get; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    private enum State {
        // Essential State for multiplayer mode
        WaitingToStart,
        CountdownToStart,
        GamePlay,
        GameOver,
    }

    private State state;
    // private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 120f;
    private bool isGamePaused = false;
    private bool[,] steppedTiles;
    private void Awake() {
        Instance = this;
        
        state = State.CountdownToStart;
    }
    
    private void Start() {
        // GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        // GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        
        gamePlayingTimer = gamePlayingTimerMax;
        VirtualReceiver.Instance.OnSteppingChanged += VirtualReceiver_OnSteppingChanged;
        
    }
    
    private void VirtualReceiver_OnSteppingChanged(object sender, EventArgs e)
    {
        // VirtualReceiver.Instance.displayFrame(new Color32[][]);
        VirtualReceiver.Instance.DoSomething();
        steppedTiles = VirtualReceiver.Instance.GetSteppedTiles();
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (state is State.WaitingToStart) {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                // waitingToStartTimer -= Time.deltaTime;
                // if (waitingToStartTimer < 0f) {
                //     state = State.CountdownToStart;
                //     OnStateChanged?.Invoke(this, EventArgs.Empty);
                // }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f) {
                    state = State.GamePlay;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlay:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f) {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
            
        }
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }
    
    public bool IsGamePlay() {
        return state == State.GamePlay;
    }
    
    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimerNormalized() {
        return gamePlayingTimer / gamePlayingTimerMax;
    }

    public void TogglePauseGame() {
        // Since this game's object and player events changes all base on Time.deltaTime
        // This approach can freeze all changes to achieve a Pause
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
        
    }
}
