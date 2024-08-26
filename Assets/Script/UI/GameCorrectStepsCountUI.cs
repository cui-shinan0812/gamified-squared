using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Remember to disable Wrapping inside text

public class GameCorrectStepsCountUI : MonoBehaviour {
    private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI correctStepsCountText;

    private Animator animator;
    private int previousCountdownNumber;
    private void Awake() {
        gameObject.SetActive(true);
        animator = correctStepsCountText.GetComponent<Animator>();
    }

    private void Start() {
        GameplayPathMemManager.Instance.OnStateChanged += GameplayPathMemManager_OnStateChanged;
        GameplayPathMemManager.Instance.OnScoreChanged += GameplayPathMemManager_OnScoreChanged;
        
        Hide();
    }

    private void GameplayPathMemManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameplayPathMemManager.Instance.IsGameplayActive()) {
            Debug.Log("GameplayPathMemManager_OnStateChanged Show");
            Show();
        } else {
            Hide();
        }
    }

    private void GameplayPathMemManager_OnScoreChanged(object sender, EventArgs e) {

        correctStepsCountText.text = GameplayPathMemManager.Instance.stepCorrectCount.ToString();
        animator.SetTrigger(NUMBER_POPUP);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }
    
}