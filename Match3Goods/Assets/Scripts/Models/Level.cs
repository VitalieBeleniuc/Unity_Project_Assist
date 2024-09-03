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
    public LevelType LevelType;

    [System.Serializable]
    public class Slot
    {
        public int PositionX; // test
        public int PositionY; // test

    }
}

public enum LevelType
{
    Normal,
    Hard,
    SuperHard
}