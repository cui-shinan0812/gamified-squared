using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameplayPathMemManager : MonoBehaviour
{
    public static GameplayPathMemManager Instance { get; private set; }
    
    private int[][][] answerMap;
    private int[][] answerMapForDisplay;
    private int[][] playerViewMap;
    private bool[][] steppedMap;
    private VirtualLedGridSO[][] colorChangeControlMap;
    public int stepCorrectCount { get; private set; }
    private int previousStepY;
    private int previousStepX;
    
    public event EventHandler OnStateChanged;
    public event EventHandler OnScoreChanged;

    private enum State
    {
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
    private float countdownToStartTimerMax = 5f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 60f;

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
        m = answerMap[theFrameToDisplay].Length;
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
                            playerViewMap[y][x] = 1;
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
                            playerViewMap[y][x] = 2;
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
                            playerViewMap[y][x] = 0;
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
                                    answerMapForDisplay[y][x] = 2;
                                    break;
                                }
                            }
                        }
                        DisplayAnswerViewMap(answerMap[theFrameToDisplay]);
                        DllManager.Instance.DisplayFrame(answerMap[theFrameToDisplay], m, n);
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
                // if (gamePlayingTimer == gamePlayingTimerMax)
                // {
                //     InitPlayerViewMap();
                // }
                gamePlayingTimer -= Time.deltaTime;

                int gameplayNumber = Mathf.CeilToInt(gamePlayingTimer);
                //DisplayPlayerViewMap();
                DllManager.Instance.DisplayFrame(playerViewMap, m, n);
                steppedMap = UDPManager.Instance.GetTempStepMap();
                ColorChangeTimeControl();
                if (previousGameplayTimeNumber != gameplayNumber)
                {
                    if (gamePlayingTimer >= 0)
                    {
                        // if (playerViewMap[previousStepY][previousStepX] == 1)
                        // {
                        //     playerViewMap[previousStepY][previousStepX] = 3;
                        // }
                        // RandomStep();
                        // FixedStep();
                        // if (playerViewMap[previousStepY][previousStepX] == 1)
                        // {
                        //     playerViewMap[previousStepY][previousStepX] = 3;
                        // }
                        Debug.Log("[System] Correct step: " + stepCorrectCount);
                    }
                    previousGameplayTimeNumber = gameplayNumber;
                }
                SteppingHandle();
                
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                if (stepCorrectCount == answerMap.Length)
                {
                    state = State.GameOver;
                }

                break;
            case State.GameOver:
                ConcludeResult();
                state = State.WaitingToStart;
                break;

        }
    }

    public void SetStepMap(bool[][] map)
    {
        steppedMap = map;
    }

    public int[][] GetOneFrameOfAnswerMap(int frameIndex)
    {
        return answerMap[frameIndex];
    }
    
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

    private void SteppingHandle()
    {
        for (int y = 0; y < playerViewMap.Length; y++)
        {
            for (int x = 0; x < playerViewMap[0].Length; x++)
            {
                if (steppedMap[y][x] && playerViewMap[y][x] != 2)
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
                            playerViewMap[y][x] = 2;
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
                                playerViewMap[y][x] = 1;
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

    private void ConcludeResult()
    {
        if (gamePlayingTimer < 0)
        {
            gamePlayingTimer = 0;

        }
        Debug.Log("[System] Time remain: " + String.Format("{0:0.00}", gamePlayingTimer));
        int correctStepsScore = 100 * stepCorrectCount;
        int timeRemainBonus = 50 * Mathf.CeilToInt(gamePlayingTimer);
        int gameTotalScore = correctStepsScore + timeRemainBonus;
        
        Debug.Log("[System] Correct Stepping Score: " + correctStepsScore);
        Debug.Log("[System] Time Remaining Bonus Score: " + timeRemainBonus);
        Debug.Log("[System] Your score: " + gameTotalScore);
    }

    private void RandomStep()
    {
        int randomY = Random.Range(0, steppedMap.Length);
        int randomX = Random.Range(0, steppedMap[0].Length);

        steppedMap[randomY][randomX] = true;
    }

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

    private void ColorChangeTimeControl()
    {
        for (int y = 0; y < colorChangeControlMap.Length; y++)
        {
            for (int x = 0; x < colorChangeControlMap[0].Length; x++)
            {
                colorChangeControlMap[y][x].SetRedToYellowCountDownTime(Time.deltaTime);

                if ( ConvertToGameplayManagerViewPixel(playerViewMap[y][x]) == EnumColor.RED && colorChangeControlMap[y][x].GetRedToYellowCountDownTime() < 0)
                {
                    playerViewMap[y][x] = 3;
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
                playerViewMap[y][x] = 4;
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
                answerMapForDisplay[y][x] = 4;
            }
        }
    }
}
