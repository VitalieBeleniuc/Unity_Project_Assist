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

    // Zenject
    [Inject]
    public void Construct(LevelPathsSO levelPaths)
    {
        _levelPaths = levelPaths;
    }
    void Start()
    {
        SetCurrentLevelIndex(0);
        LoadCurrentLevel(); 
    }
    public void LoadCurrentLevel() // Incarca nivelul
    {
        string jsonPath = _levelPaths.levelJsonPaths[_currentLevelIndex];
        string jsonContent = File.ReadAllText(jsonPath);

        // Covnertirea
        Level levelData = JsonConvert.DeserializeObject<Level>(jsonContent);

        // TODO: transform into a popup
        Debug.Log($"Level: {levelData.NumberOfLevel}, Duration: {levelData.Duration}, Difficulty: {levelData.LevelType}");

        // Setare timer
        _timerManager.StartLevelTimer(levelData.Duration);

        // Incarcare slot-uri conform pozitiilor din JSON
        _slotManager.LoadSlots(levelData.Shelves);
    }
    
    // TODO: Next level loading


    public void SetCurrentLevelIndex(int index)
    {
        _currentLevelIndex = index;
    }
    public int GetCurrentLevelIndex()
    {
        return _currentLevelIndex;
    }
}
