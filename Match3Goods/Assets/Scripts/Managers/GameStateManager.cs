using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }
    [Inject] private PopupManager popupManager;
    [Inject] private SlotManager slotManager;
    [Inject] private LevelManager levelManager;
    [Inject] private TimerManager timerManager;

    private DraggableItem[] draggableItems;

    private void Start()
    {
        // initalizare stare
        CurrentState = GameState.None;
        ChangeState(GameState.Start);

        draggableItems = FindObjectsOfType<DraggableItem>();
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return; // prevenire in caz de aceeasi stare
        }
        CurrentState = newState;

        // starile
        switch (newState)
        {
            case GameState.Start:
                OnEnterStartState();
                break;

            case GameState.Playing:
                OnEnterPlayingState();
                break;

            case GameState.Win:
                OnEnterWinState();
                break;

            case GameState.Loss:
                OnEnterLossState();
                break;

            case GameState.LevelTransition:
                OnEnterLevelTransitionState();
                break;

            case GameState.Pause:
                OnEnterPauseState();
                break;

            case GameState.GameOver:
                OnEnterGameOverState();
                break;
        }
    }

    // TODO: remove debugging logs
    private void OnEnterStartState()
    {
        Debug.Log("Game is in Start state.");
        popupManager.HideOverPopup();
        popupManager.HideLossPopup(); // pentru viitor
        levelManager.SetCurrentLevelIndex(0);
        levelManager.LoadCurrentLevel();
    }
    private void OnEnterPlayingState()
    {
        Debug.Log("Game is in Playing state.");
        Time.timeScale = 1;
        SetDraggableItems(true);
    }

    private void OnEnterWinState()
    {
        Debug.Log("Game is in Win state.");
        slotManager.ClearShelves();
        timerManager.StopLevelTimerCouroutine();
        popupManager.ShowWinPopup();
    }

    private void OnEnterLossState()
    {
        Debug.Log("Game is in Loss state.");
        slotManager.ClearShelves();
        timerManager.SetTimeZero();
        popupManager.ShowLossPopup();
    }

    private void OnEnterLevelTransitionState()
    {
        Debug.Log("Game is transitioning to the next level.");
        StartCoroutine(TransitionToNextLevel());
    }

    // pe viitor
    private void OnEnterPauseState()
    {
        Debug.Log("Game is Paused.");
        Time.timeScale = 0;
        SetDraggableItems(false);
    }

    private void OnEnterGameOverState()
    {
        Debug.Log("Game is Over.");
        popupManager.ShowOverPopup();
    }

    private IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(3.5f); // TODO: ajustare delay
        popupManager.HideWinPopup();
        levelManager.LoadNextLevel();
        ChangeState(GameState.Playing); // pentru prevenire stare win -> win
    }

    private void SetDraggableItems(bool canDrag)
    {
        foreach (var item in draggableItems)
        {
            item.canDrag = canDrag; // Enable/disable dragging pentru pauza
        }
    }

}

public enum GameState
{
    None,
    Start,
    Playing,
    Win,
    Loss,
    Pause,
    GameOver,
    LevelTransition
}