using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class TimerManager : MonoBehaviour
{
    public Text timerText;
    private float _levelTimer;
    private Coroutine levelTimerCoroutine;
    [Inject] private GameStateManager stateManager;


    // actualizare ui timer
    public void UpdateTimerDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // evitarea -01:-01 TODO: de cautat o solutie mai eleganta
    public void SetTimeZero()
    {
        timerText.text = "00:00";
    }

    public void StartLevelTimer(float duration)
    {
        _levelTimer = duration;
        levelTimerCoroutine = StartCoroutine(LevelTimer());
    }

    // stopare corutina timer
    public void StopLevelTimerCouroutine()
    {
        StopCoroutine(levelTimerCoroutine);
    }

    private IEnumerator LevelTimer()
    {
        while (_levelTimer > 0)
        {
            _levelTimer -= Time.deltaTime;
            UpdateTimerDisplay(_levelTimer);
            yield return null;
        }

        stateManager.ChangeState(GameState.Loss);
    }





}
