﻿using JetBrains.Annotations;
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
    public Dictionary<int, Dictionary<int, Path>> MasterPath;

    int MaxPath;
    int LowestTotalPath;
    List<int> Entrances;
    List<int> Exits;
    List<int> PathIds = new List<int>();

    const int MaxBranch = 8;
    int Branches;
    bool ExitFound => Branches > 0;
    bool Running = false;

    Vector2Int forward;
    Vector2Int left;
    Vector2Int diagLeft;
    Vector2Int right;
    Vector2Int diagRight;
    Vector2Int back;

    Square endSquare;

    static AiPathing instance;

    public static AiPathing Instance 
    {
        get 
        { 
            if (instance == null)
            {
                instance = new AiPathing();
            } 
            return instance;
        }
    }

    private AiPathing() { }

    public void SetEnterancesAndExits(List<Vector2Int> enterances, List<Vector2Int> exits)
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

    }

    public IEnumerator<YieldInstruction> CalculatePath()
    {
        if (Running)
        {
            yield break;
        }

        Running = true;
        MasterPath = new Dictionary<int, Dictionary<int, Path>>();

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

            if (startSquare.hasBox)
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
                endSquare = Grid.squares[j];

                head.Current = startSquare;
                head.FutureSquares = new List<Path>();

                Path newPath = new Path(current, head);
                head.FutureSquares.Add(newPath);

                forward = dirFromEntrance;
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

                if (squareNextToExit.hasBox)
                {
                    Debug.LogWarning("Exit not valid");
                    continue;
                }

                left = new Vector2Int(-forward.y, forward.x);
                diagLeft = left + forward;
                right = -left;
                diagRight = right + forward;
                back = -forward;

                DownPath(newPath, 1, dirFromEntrance);

                if (newPath.FutureSquares.Count == 0)
                {
                    Debug.LogWarning($"No routes where found to exit {endSquare.gridCoord}");
                }
                else
                {
                    currentPath.Add(j, head);
                }

                yield return null;
            }

            if (currentPath.Count != 0)
            {
                //add all paths from current entrance to all exits
                MasterPath.Add(i, currentPath);
            }
        }

        Running = false;
    }

    Path DownPath(Path currentPath, int pathInt, Vector2Int lastTime)
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
        if (pathInt + diff > MaxPath)
        {
            return null;
        }    

        PathIds.Add(GridUtilities.TwoToOne(coord));

        //squares on the edge of the map should either be boxes or
        //enterances/exits so there's no point in us checking them
        //if the straight distance to the exit will make out path too
        //long we shouldn't bother checking it either
        if (GridUtilities.IsOnEdge(current))
        {
            return null;
        }

        if(RightIsBetter(current.gridCoord, endSquare.gridCoord, right))
        {
            Vector2Int temp = right;
            right = left;
            left = temp;

            temp = diagRight;
            diagRight = diagLeft;
            diagLeft = temp;
        }

        Square forwardSquare = GridUtilities.GetNextSquare(current, forward);
        Square leftSquare = GridUtilities.GetNextSquare(current, left);
        Square diagLeftSquare = GridUtilities.GetNextSquare(current, diagLeft);
        Square diagBackLeftSquare = GridUtilities.GetNextSquare(current, -diagRight);
        Square rightSquare = GridUtilities.GetNextSquare(current, right);
        Square diagRightSquare = GridUtilities.GetNextSquare(current, diagRight);
        Square diagBackRightSquare = GridUtilities.GetNextSquare(current, -diagLeft);
        Square backSquare = GridUtilities.GetNextSquare(current, back);

        currentPath.FutureSquares = new List<Path>();

        //FORWARD
        if (back != lastTime)
        {
            CheckNextPath(ref currentPath, forwardSquare, pathInt, forward);
        }
        //LEFT
        if (CheckDirection(left, forwardSquare, diagLeftSquare, lastTime, coord))
        {
            CheckNextPath(ref currentPath, leftSquare, pathInt, left);
        }
        //RIGHT
        if (CheckDirection(right, forwardSquare, diagRightSquare, lastTime, coord))
        {
            CheckNextPath(ref currentPath, rightSquare, pathInt, right);
        }
        //BACK 
        if (forward != lastTime &&
           ((leftSquare != null && leftSquare.hasBox) ||
           (rightSquare != null && rightSquare.hasBox) ||
           (diagBackLeftSquare != null && diagBackLeftSquare.hasBox) ||
           (diagBackRightSquare != null && diagBackRightSquare.hasBox)))
        {
            CheckNextPath(ref currentPath, backSquare, pathInt, back);
        }

        PathIds.Remove(GridUtilities.TwoToOne(coord));

        if (currentPath.FutureSquares.Count > 0)
        {
            return currentPath;
        }

        return null;
    }

    bool CheckDirection (Vector2Int dir, Square boxCheckOne, Square boxCheckTwo, Vector2Int lastTime, Vector2Int coord)
    {
        return -dir != lastTime &&
           ((!ExitFound && (dir.x != 0 && coord.x != endSquare.gridCoord.x || dir.y != 0 && coord.y != endSquare.gridCoord.y)) ||
           (boxCheckOne != null && boxCheckOne.hasBox) ||
           (boxCheckTwo != null && boxCheckTwo.hasBox));
    }


    bool NextToAnExit(Square end, Square left, Square right, Square forward, Square back)
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
        
    void CheckNextPath(ref Path currentPath, Square next, int pathInt, Vector2Int dir)
    {
        if (next != null && !next.hasBox &&
            !PathIds.Contains(GridUtilities.TwoToOne(next.gridCoord)))
        {
            Path nextPath = new Path(next, currentPath);
            nextPath = DownPath(nextPath, pathInt, dir);

            if (nextPath != null)
            {
                currentPath.FutureSquares.Add(nextPath);
            }
        }
    }

    static bool RightIsBetter(Vector2Int current, Vector2Int goal, Vector2Int right)
    {
        Vector2Int toGoal = goal - current;
        int toGoalLength = Mathf.Abs(toGoal.x) + Mathf.Abs(toGoal.y);

        Vector2Int toGoalNew = goal - (current + right);
        int toGoalNewLength = Mathf.Abs(toGoalNew.x) + Mathf.Abs(toGoalNew.y);

        return toGoalNewLength < toGoalLength;
    }
}
