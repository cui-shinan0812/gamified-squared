using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameplayPathMemManager : MonoBehaviour
{
    private int[][][] answerMap;
    private int[][] playerViewMap;
    private bool[][] steppedMap;
    private int stepCorrectCount;
    private int previousStepY;
    private int previousStepX;

    public event EventHandler OnStateChanged;

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
    private float gamePlayingTimerMax = 10f;
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
    private int previousCountdownNumber;

    private void Start()
    {
        answerMap = FileManager.Instance.ReadArrayFile();
        playerViewMap = new int[answerMap[0].Length][];
        state = State.CountdownToStart;
        gamePlayingTimer = gamePlayingTimerMax;
        countdownToStartTimerMax = answerMap.Length;
        countdownToStartTimer = countdownToStartTimerMax;
        stepCorrectCount = 0;
        for (int y = 0; y < playerViewMap.Length; y++)
        {
            playerViewMap[y] = new int[answerMap[0][0].Length];
        }

        steppedMap = new bool[answerMap[0].Length][];
        
        for (int y = 0; y < steppedMap.Length; y++)
        {
            steppedMap[y] = new bool[answerMap[0][0].Length];
            for (int x = 0; x < steppedMap[0].Length; x++)
            {
                steppedMap[y][x] = false;
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
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                int countdownNumber = Mathf.CeilToInt(countdownToStartTimer);

                if (previousCountdownNumber != countdownNumber)
                {
                    previousCountdownNumber = countdownNumber;
                    Debug.Log(countdownNumber);
                    int theFrameToDisplay = -(countdownNumber - (int)countdownToStartTimerMax);
                    if (theFrameToDisplay < answerMap.Length)
                    {
                        DisplayAnswerViewMap(answerMap[theFrameToDisplay]);
                    }
                }

                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlay;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }


                break;
            case State.GamePlay:
                gamePlayingTimer -= Time.deltaTime;

                int gameplayNumber = Mathf.CeilToInt(gamePlayingTimer);
                if (previousGameplayTimeNumber != gameplayNumber)
                {
                    if (gamePlayingTimer >= 0)
                    {
                        if (playerViewMap[previousStepY][previousStepX] == 1)
                        {
                            playerViewMap[previousStepY][previousStepX] = 3;
                        }
                        RandomStep();
                        // FixedStep();
                        SteppingHandle();
                        DisplayPlayerViewMap();
                        Debug.Log("[System] Correct step: " + stepCorrectCount);
                    }
                    previousGameplayTimeNumber = gameplayNumber;
                }
                
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

    private void DisplayPlayerViewMap()
    {
        string testDisplayMap;

        if (playerViewMap.Length > 0)
        {
            for (int y = 0; y < playerViewMap.Length; y++)
            {
                testDisplayMap = "";
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

            Debug.Log("Time: " + gamePlayingTimer);
        }
    }
    
    private void DisplayAnswerViewMap(int[][] answerMap2D)
    {
        string testDisplayMap;

        if (answerMap2D.Length > 0)
        {
            for (int y = 0; y < answerMap2D.Length; y++)
            {
                testDisplayMap = "";
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

            Debug.Log("Time: " + countdownToStartTimer);
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
                    Debug.Log("-------------");
                
                    switch (ConvertToGameplayManagerViewPixel(answerMap[stepCorrectCount][y][x]))
                    {
                        case EnumColor.BLUE:
                            stepCorrectCount += 1;
                            playerViewMap[y][x] = 2;
                            Debug.Log("[System] Correct Tile!");
                            Debug.Log("[System] previousGameplayTimeNumber: "+ $"{gamePlayingTimer:0.0}");
                            Debug.Log("[System] 1 second is added!");
                            gamePlayingTimer += 1f;
                            Debug.Log("[System] gamePlayingTimer: "+ $"{gamePlayingTimer:0.0}");
                            DisplayPlayerViewMap();
                            break;
                        default:
                            playerViewMap[y][x] = 1;
                            Debug.Log("[System] Wrong Tile!");
                            Debug.Log("[System] previousGameplayTimeNumber: "+ $"{gamePlayingTimer:0.0}");
                            Debug.Log("[System] 0.5 second is reduced!");
                            gamePlayingTimer -= .5f;
                            Debug.Log("[System] gamePlayingTimer: "+ $"{gamePlayingTimer:0.0}");
                            DisplayPlayerViewMap();
                            break;
                    }
                    
                    Debug.Log("-------------");
                    ResetSteps();
                    break;
                }
            }
        }
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
}
