using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


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
    private float gamePlayingTimerMax = 4f;
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
    // private float offTileScoreAddModifier = 0f;
    
    private float greenTileHPHealModifier = 0f;
    private float redTileHPHealModifier = 0f;
    private float blueTileHPHealModifier = 1f;
    private float yellowTileHPHealModifier = 0f;
    // private float offTileHPHealModifier = 0f;
    
    private float greenTileScoreDeductModifier = 0f;
    private float redTileScoreDeductModifier = 1f;
    private float blueTileScoreDeductModifier = 0f;
    private float yellowTileScoreDeductModifier = 0f;
    // private float offTileScoreDeductModifier = 0f;
    
    private float greenTileHPDamageModifier = 0f;
    private float redTileHPDamageModifier = 1f;
    private float blueTileHPDamageModifier = 0f;
    private float yellowTileHPDamageModifier = 0f;
    // private float offTileHPDamageModifier = 0f;
    
    private int previousGameplayTimeNumber;

    

    private int[][] testPixels2D = 
    {
        new [] { 0, 1 },
        new [] { 2, 4 }
    };
    
    private bool[][] testStepped2D = 
    {
        new [] { false, false },
        new [] { false, false }
    };

    private int[][][] test3DArray;

    private int[][][] pixels3D = new int[4][][];
    

    // private ArrayList pixels3D = new ArrayList();

   
    
    private void Awake() {
        Instance = this;
        test3DArray = FileManager.Instance.ReadArrayFile();


        playerHP = maxHP;
        for (int i = 0; i < pixels3D.Length; i++)
        {
            pixels3D[i] = new int[2][];
            for (int j = 0; j < pixels3D[i].Length; j++)
            {
                pixels3D[i][j] = new int[2];
            }
        }
        

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
                
                int gameplayNumber = Mathf.CeilToInt(gamePlayingTimer);

                if (previousGameplayTimeNumber != gameplayNumber) {
                    previousGameplayTimeNumber = gameplayNumber;
                    // VirtualReceiver.Instance.displayFrame(testPixels2D);
                    Debug.Log("index: " + (int)-(gamePlayingTimer - gamePlayingTimerMax));
                    if ((int)-(gamePlayingTimer - gamePlayingTimerMax) < test3DArray.Length)
                    {
                        VirtualReceiver.Instance.displayFrame(test3DArray[(int)-(gamePlayingTimer - gamePlayingTimerMax)]);

                        Debug.Log("Player HP: " + playerHP);
                        Debug.Log("Score: " + score);
                        Debug.Log("Time: " + gameplayNumber);
                    }

                    // for (int y = 0; y < testPixels2D.Length; y++)
                    // {
                    //     for (int x = 0; x < testPixels2D[0].Length; x++)
                    //     {
                    //         testPixels2D[y][x] = Random.Range(0, 2);
                    //         if (Random.Range(0,2) == 0)
                    //         {
                    //             testPixels2D[y][x] = 4;
                    //         }
                    //     }
                    // }
                    // if (gameplayNumber == 4)
                    // {
                    //     testStepped2D[0][0] = true;
                    //     testStepped2D[0][1] = false;
                    //     testStepped2D[1][0] = false;
                    //     testStepped2D[1][1] = false;
                    //     
                    // }
                    // if (gameplayNumber == 3)
                    // {
                    //     testStepped2D[0][0] = false;
                    //     testStepped2D[0][1] = true;
                    //     testStepped2D[1][0] = false;
                    //     testStepped2D[1][1] = false;
                    //     
                    // }
                    // if (gameplayNumber == 2)
                    // {
                    //     testStepped2D[0][0] = false;
                    //     testStepped2D[0][1] = false;
                    //     testStepped2D[1][0] = true;
                    //     testStepped2D[1][1] = false;
                    //     
                    // }
                    // if (gameplayNumber == 1)
                    // {
                    //     testStepped2D[0][0] = false;
                    //     testStepped2D[0][1] = false;
                    //     testStepped2D[1][0] = false;
                    //     testStepped2D[1][1] = true;
                    //     
                    // }
                    // if (gameplayNumber == 0)
                    // {
                    //     testStepped2D[0][0] = true;
                    //     testStepped2D[0][1] = true;
                    //     testStepped2D[1][0] = false;
                    //     testStepped2D[1][1] = false;
                    //     
                    // }
                }
                if (gamePlayingTimer < 0f) {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
            
        }
    }

    private void ConcludingSteppingResult(bool[][] steppedTiles)
    {
        int[][] hardwareViewPixels = testPixels2D;

        // int height = PixelManager.Instance.GetHeight();
        // int width = PixelManager.Instance.GetWidth();

        // int changingScore = 0;
        // int changingHP = 0;

        for (int y = 0; y < hardwareViewPixels.Length; y++)
        {
            for (int x = 0; x < hardwareViewPixels[0].Length; x++)
            {
                if (steppedTiles[y][x])
                {
                    Debug.Log("stepped y,x: " + y + ", " + x);
                }
                if (steppedTiles[y][x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y][x]) == EnumColor.GREEN)
                {
                    // Do something for GREEN tiles
                    score += Mathf.CeilToInt(scoreToAdd * greenTileScoreAddModifier);
                    Debug.Log("Score Added:" + Mathf.CeilToInt(scoreToAdd * greenTileScoreAddModifier));
                    playerHP += Mathf.CeilToInt(HPToHeal * greenTileHPHealModifier);
                    
                    score += -Mathf.CeilToInt(scoreToDeduct * greenTileScoreDeductModifier);
                    playerHP += -Mathf.CeilToInt(HPToDamage * greenTileHPDamageModifier);
                    Debug.Log("Color Code: " + hardwareViewPixels[y][x]);
                }

                if (steppedTiles[y][x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y][x]) == EnumColor.RED)
                {
                    // Do something for RED tiles
                    score += Mathf.CeilToInt(scoreToAdd * redTileScoreAddModifier);
                    playerHP += Mathf.CeilToInt(HPToHeal * redTileHPHealModifier);
                    
                    score += -Mathf.CeilToInt(scoreToDeduct * redTileScoreDeductModifier);
                    Debug.Log("Score Deducted:" + Mathf.CeilToInt(scoreToDeduct * redTileScoreDeductModifier));
                    playerHP += -Mathf.CeilToInt(HPToDamage * redTileHPDamageModifier);
                    Debug.Log("Color Code: " + hardwareViewPixels[y][x]);
                }
                
                if (steppedTiles[y][x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y][x]) == EnumColor.BLUE)
                {
                    // Do something for BLUE tiles
                    score += Mathf.CeilToInt(scoreToAdd * blueTileScoreAddModifier);
                    Debug.Log("Score Added:" + Mathf.CeilToInt(scoreToAdd * blueTileScoreAddModifier));
                    playerHP += Mathf.CeilToInt(HPToHeal * blueTileHPHealModifier);
                    Debug.Log("HP Healed:" + Mathf.CeilToInt(HPToHeal * blueTileHPHealModifier));
                    
                    score += -Mathf.CeilToInt(scoreToDeduct * blueTileScoreDeductModifier);
                    playerHP += -Mathf.CeilToInt(HPToDamage * blueTileHPDamageModifier);
                    Debug.Log("Color Code: " + hardwareViewPixels[y][x]);

                }
                
                if (steppedTiles[y][x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y][x]) == EnumColor.YELLOW)
                {
                    // Do something for YELLOW tiles
                    score += Mathf.CeilToInt(scoreToAdd * yellowTileScoreAddModifier);
                    playerHP += Mathf.CeilToInt(HPToHeal * yellowTileHPHealModifier);
                    
                    score += -Mathf.CeilToInt(scoreToDeduct * yellowTileScoreDeductModifier);
                    playerHP += -Mathf.CeilToInt(HPToDamage * yellowTileHPDamageModifier);
                    Debug.Log("Color Code: " + hardwareViewPixels[y][x]);

                }
                
                if (steppedTiles[y][x] && ConvertToGameplayManagerViewPixel(hardwareViewPixels[y][x]) == EnumColor.OFF)
                {
                    // Do something for OFF tiles
                    // score = Mathf.CeilToInt(scoreToAdd * offTileScoreAddModifier);
                    // playerHP = Mathf.CeilToInt(HPToHeal * offTileHPHealModifier);
                    //
                    // score = -Mathf.CeilToInt(scoreToDeduct * offTileScoreDeductModifier);
                    // playerHP = -Mathf.CeilToInt(HPToDamage * offTileHPDamageModifier);
                }
                
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
        Debug.Log("New Score: " + score);
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
        Debug.Log("New HP: " + playerHP);
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
