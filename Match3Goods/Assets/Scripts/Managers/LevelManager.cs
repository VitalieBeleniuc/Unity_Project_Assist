using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using static Level;

public class LevelManager : MonoBehaviour
{
    private LevelPathsSO _levelPaths;
    private int _currentLevelIndex;

    [Inject] private SlotManager _slotManager;
    [Inject] private TimerManager _timerManager;
    [Inject] private GameStateManager stateManager;

    // Zenject
    [Inject]
    public void Construct(LevelPathsSO levelPaths)
    {
        _levelPaths = levelPaths;
    }

    public void LoadCurrentLevel() // Incarca nivelul
    {
        string jsonPath = _levelPaths.levelJsonPaths[_currentLevelIndex];
        string jsonContent = File.ReadAllText(jsonPath);

        // Covnertirea
        Level levelData = JsonConvert.DeserializeObject<Level>(jsonContent);
        Debug.Log($"Level: {levelData.NumberOfLevel}, Duration: {levelData.Duration}, Difficulty: {levelData.LevelType}");

        // Setare timer
        _timerManager.StartLevelTimer(levelData.Duration+1);

        // Incarcare slot-uri conform pozitiilor din JSON
        _slotManager.LoadSlots(levelData.Shelves);
    }
    
    public void LoadNextLevel()
    {
        _timerManager.StopLevelTimerCouroutine();
        if (_currentLevelIndex + 1 < _levelPaths.levelJsonPaths.Count)
        {
            _currentLevelIndex++;
            LoadCurrentLevel();
        }
        else
        {
            stateManager.ChangeState(GameState.GameOver);
        }
    }

    public void SetCurrentLevelIndex(int index)
    {
        _currentLevelIndex = index;
    }
    public int GetCurrentLevelIndex()
    {
        return _currentLevelIndex;
    }
}
