using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public Square Current;

    public Path ParentPath;
    public List<Path> FutureSquares;

    public Path() { }

    public Path(Square cur, Path parent)
    {
        Current = cur;
        ParentPath = parent;
    }
}

public class AiPathing
{
    static int MaxPath;
    static int LowestTotalPath;
    static List<int> Entrances;
    static List<int> Exits;
    public static Dictionary<int, Dictionary<int, Path>> MasterPath;


    public static Dictionary<int, Dictionary<int, Path>> CalculatePath(List<Vector2Int> enterances, List<Vector2Int> exits)
    {
        Entrances = new List<int>();
        Exits = new List<int>();

        foreach (Vector2Int coord in enterances)
        {
            Entrances.Add(GridUtilities.TwoToOne(coord));
        }

        foreach (Vector2Int coord in exits)
        {
            Exits.Add(GridUtilities.TwoToOne(coord));
        }

        return CalculatePath();
    }

    public static Dictionary<int, Dictionary<int, Path>> CalculatePath()
    {
        Dictionary<int, Dictionary<int, Path>> allPaths = new Dictionary<int, Dictionary<int, Path>>();

        foreach (int i in Entrances)
        {
            Dictionary<int, Path> currentPath = new Dictionary<int, Path>();
            foreach (int j in Exits)
            {
                MaxPath = (Grid.width - 2) * (Grid.height - 2) + 2;
                LowestTotalPath = MaxPath;

                Path head = new Path();

                Square startSquare = Grid.squares[i];
                Square endSquare = Grid.squares[j];
                Square current = startSquare;

                head.Current = startSquare;
                head.FutureSquares = new List<Path>();

                Vector2Int forward = new Vector2Int(0, 1);

                if (GridUtilities.IsLeft(i))
                {
                    forward = new Vector2Int(1, 0);
                }
                else if (GridUtilities.IsRight(i))
                {
                    forward = new Vector2Int(-1, 0);
                }
                else if (GridUtilities.IsTop(i))
                {
                    forward = new Vector2Int(0, -1);
                }
                else if (!GridUtilities.IsBottom(i))
                {
                    Debug.Log("Enterance must be on edge of board");
                }

                current = GridUtilities.GetNextSquare(current, forward);

                if (current.hasBox)
                {
                    Debug.LogError("Enterance not valid");
                }

                Path newPath = new Path(current, head);
                head.FutureSquares.Add(newPath);

                if (GridUtilities.IsLeft(j))
                {
                    forward = new Vector2Int(-1, 0);
                }
                else if (GridUtilities.IsRight(j))
                {
                    forward = new Vector2Int(1, 0);
                }
                else if (GridUtilities.IsTop(j))
                {
                    forward = new Vector2Int(0, 1);
                }
                else if (GridUtilities.IsBottom(j))
                {
                    forward = new Vector2Int(0, -1);
                }
                else
                {
                    Debug.LogError("Exit must be on edge of board");
                }

                DownPath(newPath, endSquare, forward, 1, new Vector2Int(0,0));
                if (newPath.FutureSquares.Count == 0)
                {
                    Debug.LogWarning($"No routes where found to exit {endSquare.gridCoord}");
                }
                else
                {
                    currentPath.Add(j, head);
                }
            }
             allPaths.Add(i, currentPath);
        }

        return allPaths;
    }

    static Path DownPath(Path currentPath, Square endSquare, Vector2Int forward, int pathInt, Vector2 lastTime)
    {
        pathInt++;

        if (pathInt > MaxPath)
        {
            return null;
        }

        Vector2Int coord = currentPath.Current.gridCoord;
        Square current = currentPath.Current;

        if (coord == endSquare.gridCoord)
        {
            if (pathInt < LowestTotalPath)
            {
                MaxPath = (int)(pathInt * 1.2f);
                LowestTotalPath = pathInt;
            }
            return currentPath;
        }

        Vector2Int difCoord = coord - endSquare.gridCoord;
        int diff = Mathf.Abs(difCoord.x) + Mathf.Abs(difCoord.y);

        if (GridUtilities.IsOnEdge(currentPath.Current) || pathInt + diff > MaxPath)
        {
            return null;
        }

        Vector2Int left = new Vector2Int(-forward.y, forward.x);
        Vector2Int right = -left;

        var forwardSquare = GridUtilities.GetNextSquare(current, forward);
        var leftSquare = GridUtilities.GetNextSquare(current, left);
        var rightSquare = GridUtilities.GetNextSquare(current, right);

        currentPath.FutureSquares = new List<Path>();

        if (-forward != lastTime)
        {
            CheckNextPath(ref currentPath, forwardSquare, endSquare, forward, pathInt, forward);
        }
        if (-left != lastTime)
        {
            CheckNextPath(ref currentPath, leftSquare, endSquare, forward, pathInt, left);
        }
        if (-right != lastTime)
        {
            CheckNextPath(ref currentPath, rightSquare, endSquare, forward, pathInt, right);
        }

        if (currentPath.FutureSquares.Count > 0)
        {
            return currentPath;
        }

        return null;
    }

    static void CheckNextPath(ref Path currentPath, Square next, Square endSquare, Vector2Int forward, int pathInt, Vector2Int dir)
    {
        if (next != null && !next.hasBox)
        {
            Path nextPath = new Path(next, currentPath);
            nextPath = DownPath(nextPath, endSquare, forward, pathInt, dir);

            if (nextPath != null)
            {
                currentPath.FutureSquares.Add(nextPath);
            }
        }
    }

}
