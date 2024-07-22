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

    private enum EnumColor
    {
        GREEN,
        RED,
        BLUE,
        YELLOW,
        OFF,
    }

    private State state;
    // private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 120f;
    private bool isGamePaused = false;
    
    
    private int maxHP = 100;
    private int playerHP;
    private int HPToHeal = 15;
    private int HPToDamage = 20;
    private int score;
    private int scoreToAdd = 100;
    private int scoreToDeduct = 50;
    
    private float greenTileScoreAddModifier = 1f;
    private float redTileScoreAddModifier = 0f;
    private float blueTileScoreAddModifier = .5f;
    private float yellowTileScoreAddModifier = 0f;
    private float offTileScoreAddModifier = 0f;
    
    private float greenTileHPHealModifier = 0f;
    private float redTileHPHealModifier = 0f;
    private float blueTileHPHealModifier = 1f;
    private float yellowTileHPHealModifier = 0f;
    private float offTileHPHealModifier = 0f;
    
    private float greenTileScoreDeductModifier = 0f;
    private float redTileScoreDeductModifier = 1f;
    private float blueTileScoreDeductModifier = 0f;
    private float yellowTileScoreDeductModifier = 0f;
    private float offTileScoreDeductModifier = 0f;
    
    private float greenTileHPDamageModifier = 0f;
    private float redTileHPDamageModifier = 1f;
    private float blueTileHPDamageModifier = 0f;
    private float yellowTileHPDamageModifier = 0f;
    private float offTileHPDamageModifier = 0f;
    
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
        ConcludingSteppingResult(VirtualReceiver.Instance.GetSteppedTiles());
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

    private void ConcludingSteppingResult(bool[,] steppedTiles)
    {
        int[,] hardwareViewPixels = PixelManager.Instance.GetHardwareViewPixel();

        int height = PixelManager.Instance.GetHeight();
        int width = PixelManager.Instance.GetWidth();

        int changingScore = 0;
        int changingHP = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (steppedTiles[y,x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y,x]) == EnumColor.GREEN)
                {
                    // Do something for GREEN tiles
                    changingScore = Mathf.CeilToInt(scoreToAdd * greenTileScoreAddModifier);
                    changingHP = Mathf.CeilToInt(HPToHeal * greenTileHPHealModifier);
                    
                    changingScore = -Mathf.CeilToInt(scoreToDeduct * greenTileScoreDeductModifier);
                    changingHP = -Mathf.CeilToInt(HPToDamage * greenTileHPDamageModifier);
                }

                if (steppedTiles[y,x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y,x]) == EnumColor.RED)
                {
                    // Do something for RED tiles
                    changingScore = Mathf.CeilToInt(scoreToAdd * redTileScoreAddModifier);
                    changingHP = Mathf.CeilToInt(HPToHeal * redTileHPHealModifier);
                    
                    changingScore = -Mathf.CeilToInt(scoreToDeduct * redTileScoreDeductModifier);
                    changingHP = -Mathf.CeilToInt(HPToDamage * redTileHPDamageModifier);
                }
                
                if (steppedTiles[y,x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y,x]) == EnumColor.BLUE)
                {
                    // Do something for BLUE tiles
                    changingScore = Mathf.CeilToInt(scoreToAdd * blueTileScoreAddModifier);
                    changingHP = Mathf.CeilToInt(HPToHeal * blueTileHPHealModifier);
                    
                    changingScore = -Mathf.CeilToInt(scoreToDeduct * blueTileScoreDeductModifier);
                    changingHP = -Mathf.CeilToInt(HPToDamage * blueTileHPDamageModifier);
                }
                
                if (steppedTiles[y,x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y,x]) == EnumColor.YELLOW)
                {
                    // Do something for BLUE tiles
                    changingScore = Mathf.CeilToInt(scoreToAdd * yellowTileScoreAddModifier);
                    changingHP = Mathf.CeilToInt(HPToHeal * yellowTileHPHealModifier);
                    
                    changingScore = -Mathf.CeilToInt(scoreToDeduct * yellowTileScoreDeductModifier);
                    changingHP = -Mathf.CeilToInt(HPToDamage * yellowTileHPDamageModifier);
                }
                
                if (steppedTiles[y,x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y,x]) == EnumColor.OFF)
                {
                    // Do something for BLUE tiles
                    changingScore = Mathf.CeilToInt(scoreToAdd * offTileScoreAddModifier);
                    changingHP = Mathf.CeilToInt(HPToHeal * offTileHPHealModifier);
                    
                    changingScore = -Mathf.CeilToInt(scoreToDeduct * offTileScoreDeductModifier);
                    changingHP = -Mathf.CeilToInt(HPToDamage * offTileHPDamageModifier);
                }

                score += changingScore;
                playerHP += changingHP;
                
                CheckScore();
                CheckHP();
            }
        }
    }

    private void CheckScore()
    {
        if (score < 0)
        {
            score = 0;
        }
    }

    private void CheckHP()
    {
        if (playerHP > maxHP)
        {
            playerHP = maxHP;
        }

        if (playerHP <= 0)
        {
            // Do something to end the game
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

    private EnumColor ConvertToGameplayManagerViewPixel(int colorCode)
    {
        switch (colorCode)
        {
            case 0:
                return EnumColor.GREEN;
            case 1:
                return EnumColor.RED;
            case 2:
                return EnumColor.BLUE;
            case 3:
                return EnumColor.YELLOW;
            case 4:
            default:
                return EnumColor.OFF;
        }
    }
}
