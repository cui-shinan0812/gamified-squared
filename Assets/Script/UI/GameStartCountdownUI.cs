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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        GameplayPathMemManager.Instance.OnStateChanged += GameplayPathMemManager_OnStateChanged;

        Hide();
    }

    private void GameplayPathMemManager_OnStateChanged(object sender, EventArgs e) {
        if (GameplayPathMemManager.Instance.IsCountdownToStartActive()) {
            Show();
        } else {
            // animator.ResetTrigger(NUMBER_POPUP);
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
            if (GameplayPathMemManager.Instance.IsCountdownToStartActive())
            {
                animator.SetTrigger(NUMBER_POPUP);
            }
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