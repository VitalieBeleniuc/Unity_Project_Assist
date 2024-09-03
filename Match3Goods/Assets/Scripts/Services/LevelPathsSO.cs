using System.Collections.Generic;
using UnityEngine;

// pentru creare direct in meniul de assets din unity
[CreateAssetMenu(fileName = "LevelPathsSO", menuName = "Assets/Resources/JSON_Levels")]
public class LevelPathsSO : ScriptableObject
{
    public List<string> levelJsonPaths;
}
