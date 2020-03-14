using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtilities
{
    public static int gridHeight = 0;
    public static int gridWidth = 0;

    // Convert from box ID to grid coordinates
    public static Vector2 OneToTwo(int val)
    {
        return new Vector2(val % gridWidth, val / gridHeight);
    }
    // Convert from grid coordinates to box ID
    public static int TwoToOne(Vector2 coord)
    {
        return (int)(coord.x + (coord.y * gridWidth));
    }
}
