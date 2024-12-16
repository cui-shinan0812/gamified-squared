using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

// This Gameplay Manager is created specific for the Path Memorizing game.
// This Manager responsible on the game logic handling
public class GameplayPathMemManager : MonoBehaviour
{
    public static GameplayPathMemManager Instance { get; private set; }
    
    // original answer array, 1 frame contain 1 correct grid only. The frame index play as the sequence of path
    private int[][][] answerMap;
    
    // the array for displaying answer. Should only be called under Count Down State.
    // Combine the answers from answerMap frame by frame to display a complete path
    private int[][] answerMapForDisplay;
    
    // for displaying the grid players stepped when the game is under Gameplay state
    private int[][] playerViewMap;
    
    // recording which grids player stepped at the frame and assist judging if player stepped correct grid
    // work with answerMap and playerViewMap
    private bool[][] steppedMap;
    
    // to contain the color change timer of each grid
    private VirtualLedGridSO[][] colorChangeControlMap;
    public int stepCorrectCount { get; private set; }
    private int gameTotalScore;
    
    // These 2 are variables used in test stage
    private int previousStepY;
    private int previousStepX;
    
    // store events that will be triggered when STATE changing 
    public event EventHandler OnStateChanged;
    
    // store events that will be triggered when SCORE changing
    public event EventHandler OnScoreChanged;

    private enum State
    {
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
    private float countdownToStartTimerMax = 5f; // seconds
    private float countdownToStartTimer = 3f; // seconds
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 60f; // seconds

    private int previousGameplayTimeNumber;
    private int previousCountdownNumber;
    private int theFrameToDisplay;
    private int m;
    private int n;

    private void Awake()
    {
        Instance = this;
        state = State.CountdownToStart;
        answerMap = FileManager.Instance.ReadArrayFile();
    }

    private void Start()
    {
        // height
        m = answerMap[theFrameToDisplay].Length;
        // width
        n = answerMap[theFrameToDisplay][0].Length;
        
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        gamePlayingTimer = gamePlayingTimerMax;
        countdownToStartTimerMax = answerMap.Length;
        countdownToStartTimer = countdownToStartTimerMax;
        stepCorrectCount = 0;
        theFrameToDisplay = 0;

        InitPlayerViewMap();
        InitAnswerViewMapForDisplay();

        steppedMap = new bool[answerMap[0].Length][];
        
        for (int y = 0; y < steppedMap.Length; y++)
        {
            steppedMap[y] = new bool[answerMap[0][0].Length];
            for (int x = 0; x < steppedMap[0].Length; x++)
            {
                steppedMap[y][x] = false;
            }
        }

        colorChangeControlMap = new VirtualLedGridSO[answerMap[0].Length][];
        
        for (int y = 0; y < answerMap[0].Length; y++)
        {
            colorChangeControlMap[y] = new VirtualLedGridSO[answerMap[0][0].Length];
            for (int x = 0; x < answerMap[0][0].Length; x++)
            {
                colorChangeControlMap[y][x] = new VirtualLedGridSO();
            }
        }
        
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                // waitingToStartTimer -= Time.deltaTime;
                // if (waitingToStartTimer < 0f) {
                //     state = State.CountdownToStart;
                //     OnStateChanged?.Invoke(this, EventArgs.Empty);
                // }
                if (stepCorrectCount == 0)
                {
                    for (int y = 0; y < playerViewMap.Length; y++)
                    {
                        for (int x = 0; x < playerViewMap[0].Length; x++)
                        {
                            playerViewMap[y][x] = ConvertToColorCode(EnumColor.RED);
                        }
                    }
                    DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                }
                if (stepCorrectCount > 0 && stepCorrectCount < answerMap.Length)
                {
                    for (int y = 0; y < playerViewMap.Length; y++)
                    {
                        for (int x = 0; x < playerViewMap[0].Length; x++)
                        {
                            playerViewMap[y][x] = ConvertToColorCode(EnumColor.BLUE);
                        }
                    }
                    DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                }
                if (stepCorrectCount == answerMap.Length)
                {
                    for (int y = 0; y < playerViewMap.Length; y++)
                    {
                        for (int x = 0; x < playerViewMap[0].Length; x++)
                        {
                            playerViewMap[y][x] = ConvertToColorCode(EnumColor.GREEN);
                        }
                    }
                    DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                int countdownNumber = Mathf.CeilToInt(countdownToStartTimer);

                if (previousCountdownNumber != countdownNumber)
                {
                    previousCountdownNumber = countdownNumber;
                    Debug.Log("[System] Counter Down:" + countdownNumber);
                    theFrameToDisplay = -(countdownNumber - (int)countdownToStartTimerMax); 
                    if (theFrameToDisplay < answerMap.Length)
                    {
                        for (int y = 0; y < answerMap[theFrameToDisplay].Length; y++)
                        {
                            for (int x = 0; x < answerMap[theFrameToDisplay][0].Length; x++)
                            {
                                if (ConvertToGameplayManagerViewPixel(answerMap[theFrameToDisplay][y][x]) == EnumColor.BLUE)
                                {
                                    answerMapForDisplay[y][x] = ConvertToColorCode(EnumColor.BLUE);
                                    break;
                                }
                            }
                        }
                        DisplayAnswerViewMap(answerMapForDisplay);
                        DllManager.Instance.DisplayFrame(answerMapForDisplay, m, n);
                    }
                }

                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlay;
                    // DisplayPlayerViewMap();
                    DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            
            case State.GamePlay:
                gamePlayingTimer -= Time.deltaTime;

                int gameplayNumber = Mathf.CeilToInt(gamePlayingTimer);
                //DisplayPlayerViewMap();
                DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                
                // VVV comment this when not connect to hardware VVV
                
                // steppedMap = UDPManager.Instance.GetTempStepMap();
                
                // ^^^ comment this when not connect to hardware ^^^
                
                ColorChangeTimeControl();
                if (previousGameplayTimeNumber != gameplayNumber)
                {
                    if (gamePlayingTimer >= 0)
                    {
                        // testing method calls. Their function conflict to each others, choose only one to active at a time
                        RandomStep();
                        // FixedStep();
                        
                        Debug.Log("[System] Correct step: " + stepCorrectCount);
                    }
                    previousGameplayTimeNumber = gameplayNumber;
                }
                SteppingHandle();
                
                if (gamePlayingTimer < 0f || stepCorrectCount == answerMap.Length)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GameOver:
                ConcludeResult();
                state = State.WaitingToStart;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;

        }
    }
    
    // DisplayPlayerViewMap is for debug and responsible for display the array Player View Map on debug console in Unity
    private void DisplayPlayerViewMap()
    {
        string testDisplayMap;

        if (playerViewMap.Length > 0)
        {
            for (int y = 0; y < playerViewMap.Length; y++)
            {
                testDisplayMap = "[System] ";
                testDisplayMap += "[";
                for (int x = 0; x < playerViewMap[0].Length; x++)
                {
                    testDisplayMap += " " + playerViewMap[y][x];
                }

                if (y < playerViewMap.Length - 1)
                {
                    testDisplayMap += " ], \n";
                }
                else
                {
                    testDisplayMap += " ] \n";
                }
                Debug.Log(testDisplayMap);
            }

            Debug.Log("[System] Time: " + gamePlayingTimer);
        }
    }
    
    // Similar to DisplayPlayerViewMap() but this display answer map instead
    private void DisplayAnswerViewMap(int[][] answerMap2D)
    {
        string testDisplayMap;

        if (answerMap2D.Length > 0)
        {
            for (int y = 0; y < answerMap2D.Length; y++)
            {
                testDisplayMap = "[System] ";
                testDisplayMap += "[";
                for (int x = 0; x < answerMap2D[0].Length; x++)
                {
                    testDisplayMap += " " + answerMap2D[y][x];
                }

                if (y < answerMap2D.Length - 1)
                {
                    testDisplayMap += " ], \n";
                }
                else
                {
                    testDisplayMap += " ] \n";
                }
                Debug.Log(testDisplayMap);
            }

            Debug.Log("[System] Time: " + countdownToStartTimer);
        }
    }

    // judge and decide the behavior and result of the current steps returned from the receiver
    private void SteppingHandle()
    {
        for (int y = 0; y < playerViewMap.Length; y++)
        {
            for (int x = 0; x < playerViewMap[0].Length; x++)
            {
                // ignore grids that are stepped correctly
                if (steppedMap[y][x] && playerViewMap[y][x] != ConvertToColorCode(EnumColor.BLUE))
                {
                    previousStepY = y;
                    previousStepX = x;
                    Debug.Log("[System] stepped y,x: " + y + ", " + x);
                    Debug.Log("[System] -------------");
                
                    switch (ConvertToGameplayManagerViewPixel(answerMap[stepCorrectCount][y][x]))
                    {
                        case EnumColor.BLUE:
                            stepCorrectCount += 1;
                            OnScoreChanged?.Invoke(this, EventArgs.Empty);
                            playerViewMap[y][x] = ConvertToColorCode(EnumColor.BLUE); // turns blue
                            Debug.Log("[System] Correct Tile!");
                            Debug.Log("[System] previousGameplayTimeNumber: "+ $"{gamePlayingTimer:0.0}");
                            Debug.Log("[System] 1 second is added!");
                            gamePlayingTimer += 1f;
                            Debug.Log("[System] gamePlayingTimer: "+ $"{gamePlayingTimer:0.0}");
                            Debug.Log("[System] Blue!");
                            //DisplayPlayerViewMap();
                            Debug.Log("[System] Blue! " + playerViewMap[y][x]);
                            break;
                        case EnumColor.OFF:
                            if (ConvertToGameplayManagerViewPixel(playerViewMap[y][x]) == EnumColor.YELLOW || 
                                ConvertToGameplayManagerViewPixel(playerViewMap[y][x]) == EnumColor.OFF)
                            {
                                playerViewMap[y][x] = ConvertToColorCode(EnumColor.RED); // turns red
                                colorChangeControlMap[y][x].InitObject();
                                Debug.Log("[System] Wrong Tile!");
                                Debug.Log("[System] previousGameplayTimeNumber: "+ $"{gamePlayingTimer:0.0}");
                                Debug.Log("[System] 0.5 second is reduced!");
                                gamePlayingTimer -= .5f;
                                Debug.Log("[System] gamePlayingTimer: "+ $"{gamePlayingTimer:0.0}");
                                //DisplayPlayerViewMap();   
                            }
                            break;
                        default:
                            break;
                    }
                    DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                    Debug.Log("[System] -------------");
                }
            }
        }
        ResetSteps();
    }

    // ConcludeResult() concludes the score of the player when Game over
    private void ConcludeResult()
    {
        if (gamePlayingTimer < 0)
        {
            gamePlayingTimer = 0;

        }
        Debug.Log("[System] Time remain: " + String.Format("{0:0.00}", gamePlayingTimer));
        int correctStepsScore = 100 * stepCorrectCount;
        int timeRemainBonus = 50 * Mathf.CeilToInt(gamePlayingTimer);
        gameTotalScore = correctStepsScore + timeRemainBonus;
        
        Debug.Log("[System] Correct Stepping Score: " + correctStepsScore);
        Debug.Log("[System] Time Remaining Bonus Score: " + timeRemainBonus);
        Debug.Log("[System] Your score: " + gameTotalScore);
    }

    // test method for random steps
    private void RandomStep()
    {
        int randomY = Random.Range(0, steppedMap.Length);
        int randomX = Random.Range(0, steppedMap[0].Length);

        steppedMap[randomY][randomX] = true;
    }

    // reset the local steps array to all false
    private void ResetSteps()
    {
        for (int y = 0; y < steppedMap.Length; y++)
        {
            for (int x = 0; x < steppedMap[0].Length; x++)
            {
                steppedMap[y][x] = false;
            }
        }
    }

    // test method for fixed (all correct) steps
    private void FixedStep()
    {
        for (int y = 0; y < answerMap[0].Length; y++)
        {
            for (int x = 0; x < answerMap[0][0].Length; x++)
            {
                if (answerMap[stepCorrectCount][y][x] == 2)
                {
                    steppedMap[y][x] = true;
                }
            }
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
    
    private int ConvertToColorCode(EnumColor enumColor)
    {
        switch (enumColor)
        {
            case EnumColor.GREEN:
                return 0;
            case EnumColor.RED:
                return 1;
            case EnumColor.BLUE:
                return 2;
            case EnumColor.YELLOW:
                return 3;
            case EnumColor.OFF:
            default:
                return 4;
        }
    }

    // Wrong stepping behaviour control
    private void ColorChangeTimeControl()
    {
        for (int y = 0; y < colorChangeControlMap.Length; y++)
        {
            for (int x = 0; x < colorChangeControlMap[0].Length; x++)
            {
                colorChangeControlMap[y][x].SetRedToYellowCountDownTime(Time.deltaTime);

                if ( ConvertToGameplayManagerViewPixel(playerViewMap[y][x]) == EnumColor.RED && colorChangeControlMap[y][x].GetRedToYellowCountDownTime() < 0)
                {
                    playerViewMap[y][x] = ConvertToColorCode(EnumColor.YELLOW);
                }
            }
        }
    }
    
    public float GetGamePlayingTimerNormalized() {
        return gamePlayingTimer / gamePlayingTimerMax;
    }
    
    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }
    
    public bool IsGameplayActive() {
        return state == State.GamePlay;
    }

    public bool IsWaitingToStart()
    {
        return state == State.WaitingToStart;
    }
    
    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }
    
    private void InitPlayerViewMap()
    {
        playerViewMap = new int[answerMap[0].Length][];
        for (int y = 0; y < playerViewMap.Length; y++)
        {
            playerViewMap[y] = new int[answerMap[0][0].Length];
            for (int x = 0; x < playerViewMap[0].Length; x++)
            {
                playerViewMap[y][x] = ConvertToColorCode(EnumColor.OFF);
            }
        }
    }
    
    private void InitAnswerViewMapForDisplay()
    {
        answerMapForDisplay = new int[answerMap[0].Length][];
        for (int y = 0; y < answerMapForDisplay.Length; y++)
        {
            answerMapForDisplay[y] = new int[answerMap[0][0].Length];
            for (int x = 0; x < answerMapForDisplay[0].Length; x++)
            {
                answerMapForDisplay[y][x] = ConvertToColorCode(EnumColor.OFF);
            }
        }
    }

    public int GetGameTotalScore()
    {
        return gameTotalScore;
    }

    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer;
    }

    public int GetStepCorrectCount()
    {
        return stepCorrectCount;
    }
}
