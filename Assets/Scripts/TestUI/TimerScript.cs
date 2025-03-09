using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    void Update() {
        if (remainingTime > 0) {
            // if (remainingTime == x) Fai azione al tempo scelto
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0) {
            remainingTime = 60; // Quando finisce il timer si resetta
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
