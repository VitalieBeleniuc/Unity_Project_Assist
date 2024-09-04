using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class LevelManager : MonoBehaviour
{
    private LevelPathsSO _levelPaths;
    private int _currentLevelIndex;
    private float _levelTimer;

    public GameObject slotPrefab; // slot-ul
    public Transform slotsParent; // canvas-ul

    public GameObject redPotionPrefab;
    public GameObject bluePotionPrefab;
    public GameObject greenPotionPrefab;
    public GameObject purplePotionPrefab;

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

        // Debug.Log(levelData.Slots[1].ItemHeld); // TODO: remove
        Debug.Log($"Level: {levelData.NumberOfLevel}, Duration: {levelData.Duration}, Difficulty: {levelData.LevelType}");

        // Setare timer
        _levelTimer = levelData.Duration;
        StartCoroutine(LevelTimer());

        // Incarcare slot-uri conform pozitiilor din JSON
        LoadSlots(levelData.Slots);
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

    private void LoadSlots(List<Level.Slot> slots) // Incarca sloturile din JSON
    {

        foreach (Level.Slot slotData in slots)
        {
            slotsParent.position = Vector3.zero; // pentru mentinerea spatiului local
            GameObject slotInstance = Instantiate(slotPrefab, slotsParent);

            // pozitiile conform datelor din json
            slotInstance.transform.position = new Vector3(slotData.PositionX, slotData.PositionY, 0);

            // item-ele conform datelor din json
            if (slotData.ItemHeld != ItemType.None)
            {
                GameObject itemPrefab = GetItemPrefab(slotData.ItemHeld);
                if (itemPrefab != null)
                {
                    GameObject itemInstance = Instantiate(itemPrefab, slotInstance.transform);
                }
            }

            //Debug.Log($"Slot created at PositionX: {slotData.PositionX}, PositionY: {slotData.PositionY}, Item: {slotData.ItemHeld}");
        }
    }


    private GameObject GetItemPrefab(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.RedPotion:
                return redPotionPrefab;
            case ItemType.GreenPotion:
                return greenPotionPrefab;
            case ItemType.BluePotion:
                return bluePotionPrefab;
            case ItemType.PurplePotion:
                return purplePotionPrefab;
            default:
                return null;
        }
    }







}
