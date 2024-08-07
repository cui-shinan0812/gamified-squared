using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Remember to disable Wrapping inside text

public class GameStartCountdownUI : MonoBehaviour {
    private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;
    private int previousCountdownNumber;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        GameplayPathMemManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        

        // Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e) {
        if (GameplayPathMemManager.Instance.IsCountdownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        // ToString("#") or ToString("N0") to format the output text to integer
        // Mathf.Ceil() returns the smallest number that greater or equal to the input float number, better for countdown
        int countdownNumber = Mathf.CeilToInt(GameplayPathMemManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if (previousCountdownNumber != countdownNumber) {
            previousCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POPUP);
            // SoundManager.Instance.PlayCountdownSound();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }
    
}