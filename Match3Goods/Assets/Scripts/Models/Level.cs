using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int NumberOfLevel;
    public int Duration;
    public List<Slot> Slots;
    public LevelType LevelType; // dificultatea

    [System.Serializable]
    public class Slot
    {
        public float PositionX;
        public float PositionY;
        public ItemType ItemHeld; // item-ul pe care o sa-l contina slot-ul

    }
}

public enum LevelType
{
    Normal,
    Hard,
    SuperHard
}

public enum ItemType
{
    None, 
    RedPotion,
    GreenPotion,
    BluePotion, 
    PurplePotion
}