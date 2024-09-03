using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.IO;

public class LevelManager : MonoBehaviour
{
    private LevelPathsSO _levelPaths;
    private int _currentLevelIndex;
    private float _levelTimer;

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
        Level levelData = JsonUtility.FromJson<Level>(jsonContent);

        Debug.Log($"Level: {levelData.NumberOfLevel}, Duration: {levelData.Duration}, Difficulty: {levelData.LevelType}");

        // Setare timer
        _levelTimer = levelData.Duration;
        StartCoroutine(LevelTimer());
    }


    private IEnumerator LevelTimer() // TODO: potentiala violare de Single Responsibility Principle?
    {
        while (_levelTimer > 0)
        {
            _levelTimer -= Time.deltaTime;
            TimerManager.Instance.UpdateTimerDisplay(_levelTimer); // Actualizeaza elementul UI
            yield return null;
        }

        Debug.Log("Game Over");
        // TODO: popup pentru restartarea nivelului.
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
