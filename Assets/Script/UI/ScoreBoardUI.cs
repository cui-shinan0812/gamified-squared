using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] private GameObject starMiddle;
    [SerializeField] private GameObject starLeft;
    [SerializeField] private GameObject starRight;

    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        starLeft.gameObject.SetActive(false);
        starMiddle.gameObject.SetActive(false);
        starRight.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameplayPathMemManager.Instance.OnStateChanged += GameplayPathMemManager_OnStateChanged;
    }
    
    private void GameplayPathMemManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameplayPathMemManager.Instance.IsWaitingToStart()) {
            Debug.Log("GameplayPathMemManager_OnStateChanged Show");
            Show();
        } else {
            Hide();
        }

        timeRemainingText.text = Mathf.CeilToInt(GameplayPathMemManager.Instance.GetGamePlayingTimer()) + "";
        int totalScore = Mathf.CeilToInt(GameplayPathMemManager.Instance.GetGameTotalScore());
        scoreText.text = totalScore + "";
        StarDisplayJudgement(totalScore);
    }

    private void StarDisplayJudgement(int totalScore)
    {
        if (totalScore >= 1000 || GameplayPathMemManager.Instance.GetStepCorrectCount() > 0)
        {
            starMiddle.gameObject.SetActive(true);
        }
        
        if (totalScore >= 2000)
        {
            starLeft.gameObject.SetActive(true);
        }
        
        if (totalScore >= 3000)
        {
            starRight.gameObject.SetActive(true);
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }
}
