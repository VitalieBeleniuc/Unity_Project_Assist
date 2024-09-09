using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class ButtonFunctionality : MonoBehaviour
{
    public Button playAgainButton;
    public Button exitButton;
    [Inject] private GameStateManager stateManager;
    private void Start()
    {
        // adaugare listener-e
        playAgainButton.onClick.AddListener(PlayAgainButton);
        exitButton.onClick.AddListener(ExitButton);
    }

    private void PlayAgainButton()
    {
        stateManager.ChangeState(GameState.Start);
    }

    private void ExitButton()
    {
        Debug.Log("Exit Button Clicked!");
        Application.Quit();

        // simulare iesire aplicatie in editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
