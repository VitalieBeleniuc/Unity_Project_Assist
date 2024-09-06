using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    public Text timerText;
    private float _levelTimer;

    public void UpdateTimerDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelTimer(float duration)
    {
        _levelTimer = duration;
        StartCoroutine(LevelTimer());
    }

    private IEnumerator LevelTimer()
    {
        while (_levelTimer > 0)
        {
            _levelTimer -= Time.deltaTime;
            UpdateTimerDisplay(_levelTimer);
            yield return null;
        }

        Debug.Log("Game Over");
        // TODO: popup pentru restartarea nivelului.
    }

}
