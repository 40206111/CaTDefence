using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Path
{
    Square current;

    Square parentSquare;
    List<Square> futureSquares;
}

public class AiPathing
{
    public static Dictionary<int, Dictionary<int, Path>> CalculatePath(List<Vector2Int> enterances, List<Vector2Int> exits)
    {
        List<int> enterancesIndex = new List<int>();
        List<int> exitsIndex = new List<int>();

        foreach (Vector2Int coord in enterances)
        {
            enterancesIndex.Add(GridUtilities.TwoToOne(coord));
        }

        foreach (Vector2Int coord in exits)
        {
            exitsIndex.Add(GridUtilities.TwoToOne(coord));
        }

        return CalculatePath(enterancesIndex, exitsIndex);
    }

    public static Dictionary<int, Dictionary<int, Path>> CalculatePath(List<int> enterances, List<int> exits)
    {
        Dictionary<int, Dictionary<int, Path>> allPaths = new Dictionary<int, Dictionary<int, Path>>();

        foreach (int i in enterances)
        {
            foreach (int j in exits)
            {
                List<Square> currentPath = new List<Square>();

                Square startSquare = Grid.squares[i];
                Square endSquare = Grid.squares[j];
                Square current = startSquare;


            }
        }
                     
        return allPaths;
    }

}
