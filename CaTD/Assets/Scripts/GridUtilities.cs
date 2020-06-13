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

    public static bool IsLeft(Vector2Int coord)
    {
        return coord.x == 0;
    }

    public static bool IsLeft(int val)
    {
        return IsLeft(OneToTwo(val));
    }

    public static bool IsRight(Vector2Int coord)
    {
        return coord.x == Grid.width - 1;
    }

    public static bool IsRight(int val)
    {
        return IsRight(OneToTwo(val));
    }

    public static bool IsTop(Vector2Int coord)
    {
        return coord.y == Grid.height - 1;
    }

    public static bool IsTop(int val)
    {
        return IsTop(OneToTwo(val));
    }

    public static bool IsBottom(Vector2Int coord)
    {
        return coord.y == 0;
    }

    public static bool IsBottom(int val)
    {
        return IsBottom(OneToTwo(val));
    }

    public static Square GetNextSquare(Square current, Vector2Int dir)
    {
        Vector2Int coord = current.gridCoord;
        coord += dir;

        if (coord.x >= gridWidth || coord.y >= gridHeight
            || coord.x < 0 || coord.y < 0)
        {
            return null;
        }
        return Grid.squares[TwoToOne(coord)];
    }

    public static bool IsOnEdge(Square square)
    {
        bool output = square.gridCoord.x == gridWidth - 1;
        output |= square.gridCoord.y == gridHeight - 1;
        output |= square.gridCoord.x == 0;
        output |= square.gridCoord.y == 0;

        return output;
    }

    public static bool IsCorner(Square square)
    {
        Vector2Int coords = square.gridCoord;

        return coords == Grid.TopCorner.gridCoord ||
               coords == Grid.BottomCorner.gridCoord ||
               coords == Grid.LeftCorner.gridCoord ||
               coords == Grid.RightCorner.gridCoord;

    }
}
