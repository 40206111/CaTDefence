using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{

    public List<Vector2Int> Enterances;
    public List<Vector2Int> Exits;

    public int width;
    public int height;

    public uint TotalWaves;

    //level difficulty
    //allowed towers (by default all)
    //enemies and their wave unlock level?

}

[Serializable]
public class AllLevels
{
    public List<LevelData> Levels;

    public void Init()
    {
        if (Levels == null)
        {
            Levels = new List<LevelData>();
        }
    }

    public void AddLevel()
    {
        var newLevel = new LevelData();

        Levels.Add(newLevel);
    }

}
