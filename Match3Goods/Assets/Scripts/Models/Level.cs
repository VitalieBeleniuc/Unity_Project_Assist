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
    public List<Shelf> Shelves;
    public LevelType LevelType; // dificultatea

    [System.Serializable]
    public class Shelf // adaugat pentru fidelitate dupa logica jocului original
    {
        public float ShelfPositionX;
        public float ShelfPositionY;
        public List<Slot> Slots;

        [System.Serializable]
        public class Slot
        {
            public float SlotPositionX;
            public float SlotPositionY;
            public ItemType ItemHeld;
            public ItemType ItemHeldLayer2;
            public ItemType ItemHeldLayer3;
        }
    }
}

public enum LevelType
{
    Easy,
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
    PurplePotion,
    RedGem,
    GreenGem,
    BlueGem,
    Crown,
    GoldCoin,
    BronzeCoin,
    BronzeRing,
    Bread,
    Clover,
    Pills,
    Hourglass,
    Book,
    Bomb,
    Sword,
    Feather,
    Apple
}