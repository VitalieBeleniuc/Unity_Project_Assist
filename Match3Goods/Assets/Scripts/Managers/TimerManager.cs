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
    private Coroutine pulseCoroutine;
    private bool isPulsating;
    private bool isPaused = false;

    [Inject] private GameStateManager stateManager;

    public Button pauseButton;
    public Sprite pauseIcon;
    public Sprite playIcon;


    private void Start()
    {
        pauseButton.onClick.AddListener(TogglePause); // listener pauza buton
    }

    // actualizare ui timer
    public void UpdateTimerDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 30 && !isPulsating)
        {
            StartPulsating();
        }
        else if (timeRemaining > 30 && isPulsating) // 
        {
            StopPulsating();
        }
    }

    // evitarea -01:-01
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
        StopPulsating();
    }

    private IEnumerator LevelTimer()
    {
        while (_levelTimer > 0)
        {
            if (!isPaused) // timerul scade doar cand jocul nu e pe pauza
            {
                _levelTimer -= Time.deltaTime;
                UpdateTimerDisplay(_levelTimer);
            }
            yield return null;
        }

        StopPulsating();
        stateManager.ChangeState(GameState.Loss);
    }





    private void StartPulsating()
    {
        isPulsating = true;
        pulseCoroutine = StartCoroutine(PulsateText());
    }

    // stop efect pulsare
    private void StopPulsating()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
        isPulsating = false;
        timerText.color = Color.black;
    }

    // corutina pentru text pulsare
    private IEnumerator PulsateText()
    {
        float pulseSpeed = 1f;
        while (true)
        {
            // alternare culoare - rosu si negru
            //timerText.color = Color.Lerp(Color.red, Color.black, Mathf.PingPong(Time.time * pulseSpeed, 1));
            timerText.color = Color.red;

            // scale - marire si micsorare
            float scale = 0.40f + Mathf.PingPong(Time.time * pulseSpeed, 0.2f);
            timerText.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }
    }




    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
            stateManager.ChangeState(GameState.Playing);
        }
        else
        {
            PauseGame();
            stateManager.ChangeState(GameState.Pause);
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // Freeze game time
        pauseButton.image.sprite = playIcon;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Resume game time
        pauseButton.image.sprite = pauseIcon;
    }

}
