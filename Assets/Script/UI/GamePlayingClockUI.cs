using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour {

    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI playTimeText;

    private void Update() {
        timerImage.fillAmount = GameplayPathMemManager.Instance.GetGamePlayingTimerNormalized();
        playTimeText.text = Mathf.CeilToInt(GameplayPathMemManager.Instance.GetGamePlayingTimer()) + "";
        if (timerImage.fillAmount is < .5f and >= .25f) {
            // Half of the time passed
            timerImage.GetComponent<Image>().color = new Color32(255, 178, 90, 255);
            playTimeText.color = new Color32(255, 178, 90, 255);
        } else if (timerImage.fillAmount is < .25f) {
            // 3/4 of the time passed
            timerImage.GetComponent<Image>().color = new Color32(255, 92, 90, 255);
            playTimeText.color = new Color32(255, 92, 90, 255);
        } else {
            // default color of time
            timerImage.GetComponent<Image>().color = new Color32(219, 255, 90, 255);
            playTimeText.color = new Color32(219, 255, 90, 255);
        }
    }
}