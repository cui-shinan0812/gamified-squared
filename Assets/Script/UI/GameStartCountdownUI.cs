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
    }

    private void GameplayPathMemManager_OnStateChanged(object sender, EventArgs e) {
        if (GameplayPathMemManager.Instance.IsCountdownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        
        int countdownNumber = Mathf.CeilToInt(GameplayPathMemManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if (previousCountdownNumber != countdownNumber) {
            previousCountdownNumber = countdownNumber;
            if (GameplayPathMemManager.Instance.IsCountdownToStartActive())
            {
                // Trigger the animation when the game is under Count Down State
                animator.SetTrigger(NUMBER_POPUP);
            }
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }
    
}