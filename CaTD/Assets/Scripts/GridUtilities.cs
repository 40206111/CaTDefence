using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtilities
{
    public static int gridHeight => Grid.height;
    public static int gridWidth => Grid.width;

    // Convert from box ID to grid coordinates
    public static Vector2Int OneToTwo(int val)
    {
        return new Vector2Int(val % gridWidth, val / gridWidth);
    }

    // Convert from grid coordinates to box ID
    public static int TwoToOne(Vector2Int coord)
    {
        return (int)(coord.x + (coord.y * gridWidth));
    }
}
