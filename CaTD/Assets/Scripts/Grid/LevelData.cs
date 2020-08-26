using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    private static int nextID = 0;
    public int LevelID;
    public string LevelName;

    public LevelData(string name)
    {
        LevelName = name;
        LevelID = nextID;
        nextID++;
    }


    Vector2Int GridDimensions;

    List<Vector2Int> Enterances;
    List<Vector2Int> Exits;
}
