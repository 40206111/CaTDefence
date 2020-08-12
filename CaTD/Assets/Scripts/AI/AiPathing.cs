using JetBrains.Annotations;
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
    static List<int> PathIds = new List<int>();

    const int MaxBranch = 8;
    static int Branches;
    static bool ExitFound => Branches > 0;

    public static void CalculatePath(List<Vector2Int> enterances, List<Vector2Int> exits)
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

        MasterPath = CalculatePath();
    }

    public static Dictionary<int, Dictionary<int, Path>> CalculatePath()
    {
        Dictionary<int, Dictionary<int, Path>> allPaths = new Dictionary<int, Dictionary<int, Path>>();

        //for each entrance find path to every exit
        foreach (int i in Entrances)
        {
            Square startSquare = Grid.squares[i];
            Vector2Int dirFromEntrance = new Vector2Int(0, 1);

            if (GridUtilities.IsLeft(i))
            {
                dirFromEntrance = new Vector2Int(1, 0);
            }
            else if (GridUtilities.IsRight(i))
            {
                dirFromEntrance = new Vector2Int(-1, 0);
            }
            else if (GridUtilities.IsTop(i))
            {
                dirFromEntrance = new Vector2Int(0, -1);
            }
            else if (!GridUtilities.IsBottom(i))
            {
                Debug.Log("Enterance must be on edge of board");
            }

            startSquare = GridUtilities.GetNextSquare(startSquare, dirFromEntrance);

            if (startSquare.HasBox)
            {
                Debug.LogWarning("Enterance not valid");
                continue;
            }

            Dictionary<int, Path> currentPath = new Dictionary<int, Path>();
            foreach (int j in Exits)
            {
                Branches = 0;
                MaxPath = (Grid.width - 2) * (Grid.height - 2) + 2;
                LowestTotalPath = MaxPath;

                PathIds = new List<int>();

                Path head = new Path();

                Square current = startSquare;
                Square endSquare = Grid.squares[j];

                head.Current = startSquare;
                head.FutureSquares = new List<Path>();

                Path newPath = new Path(current, head);
                head.FutureSquares.Add(newPath);

                Vector2Int forward = dirFromEntrance;
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

                Square squareNextToExit = GridUtilities.GetNextSquare(endSquare, -forward);

                if (squareNextToExit.HasBox)
                {
                    Debug.LogWarning("Exit not valid");
                    continue;
                }

                DownPath(newPath, endSquare, forward, 1, dirFromEntrance);

                if (newPath.FutureSquares.Count == 0)
                {
                    Debug.LogWarning($"No routes where found to exit {endSquare.gridCoord}");
                }
                else
                {
                    currentPath.Add(j, head);
                }
            }

            if (currentPath.Count != 0)
            {
                //add all paths from current entrance to all exits
                allPaths.Add(i, currentPath);
            }
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
        if (Branches >= MaxBranch)
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
            Branches++;
            return currentPath;
        }

        Vector2Int difCoord = coord - endSquare.gridCoord;
        int diff = Mathf.Abs(difCoord.x) + Mathf.Abs(difCoord.y);

        PathIds.Add(GridUtilities.TwoToOne(coord));

        //squares on the edge of the map should either be boxes or
        //enterances/exits so there's no point in us checking them
        //if the straight distance to the exit will make out path too
        //long we shouldn't bother checking it either
        if (GridUtilities.IsOnEdge(current) || pathInt + diff > MaxPath)
        {
            return null;
        }

        Vector2Int left = new Vector2Int(-forward.y, forward.x);
        Vector2Int diagLeft = left + forward;
        Vector2Int right = -left;
        Vector2Int diagRight = right + forward;
        Vector2Int back = -forward;

        var forwardSquare = GridUtilities.GetNextSquare(current, forward);
        var leftSquare = GridUtilities.GetNextSquare(current, left);
        var diagLeftSquare = GridUtilities.GetNextSquare(current, diagLeft);
        var rightSquare = GridUtilities.GetNextSquare(current, right);
        var diagRightSquare = GridUtilities.GetNextSquare(current, diagRight);
        var backSquare = GridUtilities.GetNextSquare(current, back);

        currentPath.FutureSquares = new List<Path>();

        //FORWARD
        if (back != lastTime)
        {
            CheckNextPath(ref currentPath, forwardSquare, endSquare, forward, pathInt, forward);
        }
        //LEFT
        if (right != lastTime && 
           ((!ExitFound && (left.x != 0 && coord.x != endSquare.gridCoord.x || left.y != 0 && coord.y != endSquare.gridCoord.y)) ||
           (forwardSquare != null && forwardSquare.HasBox) ||
           (diagLeftSquare != null && diagLeftSquare.HasBox)))
        {
            CheckNextPath(ref currentPath, leftSquare, endSquare, forward, pathInt, left);
        }
        //RIGHT
        if (left != lastTime &&
           ((!ExitFound && (right.x != 0 && coord.x != endSquare.gridCoord.x || right.y != 0 && coord.y != endSquare.gridCoord.y)) ||
           (forwardSquare != null && forwardSquare.HasBox) ||
           (diagRightSquare != null && diagRightSquare.HasBox)))
        {
            CheckNextPath(ref currentPath, rightSquare, endSquare, forward, pathInt, right);
        }
        //BACK 
        if (forward != lastTime &&
           ((leftSquare != null && leftSquare.HasBox) ||
           (rightSquare != null && rightSquare.HasBox)))
        {
            CheckNextPath(ref currentPath, backSquare, endSquare, forward, pathInt, back);
        }

        PathIds.Remove(GridUtilities.TwoToOne(coord));

        if (currentPath.FutureSquares.Count > 0)
        {
            return currentPath;
        }

        return null;
    }

    static bool NextToAnExit(Square end, Square left, Square right, Square forward, Square back)
    {
        bool output = false;

        output |= left?.gridCoord == end.gridCoord;
        output |= right?.gridCoord == end.gridCoord;
        output |= forward?.gridCoord == end.gridCoord;
        output |= back?.gridCoord == end.gridCoord;

        if (output) { return false;  }

        output |= left != null && left.edge && Exits.Contains(GridUtilities.TwoToOne(left.gridCoord));
        output |= right != null && right.edge && Exits.Contains(GridUtilities.TwoToOne(right.gridCoord));
        output |= forward != null && forward.edge && Exits.Contains(GridUtilities.TwoToOne(forward.gridCoord));
        output |= back != null && back.edge && Exits.Contains(GridUtilities.TwoToOne(back.gridCoord));

        return output;
    }
        
    static void CheckNextPath(ref Path currentPath, Square next, Square endSquare, Vector2Int forward, int pathInt, Vector2Int dir)
    {
        if (next != null && !next.HasBox &&
            !PathIds.Contains(GridUtilities.TwoToOne(next.gridCoord)))
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
