using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiJunctions
{
    static AiJunctions instance;

    public static AiJunctions Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AiJunctions();
            }
            return instance;
        }
    }

    private AiJunctions() { }

    public Dictionary<int, Dictionary<int, Junction>> MasterPath;

    List<int> Entrances;
    List<int> Exits;

    bool Running = false;

    int MaxPath;
    int LowestTotalPath;

    public class Junction
    {
        List<Route> routes;
    }

    public class Route
    {
        List<Square> path;
        int pathValue = 0;
    }

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

        foreach (int enteranceIndex in Entrances)
        {
            Square startSquare = Grid.squares[enteranceIndex];
            Vector2Int dirFromEntrance = new Vector2Int(0, 1);

            if (GridUtilities.IsLeft(enteranceIndex))
            {
                dirFromEntrance = new Vector2Int(1, 0);
            }
            else if (GridUtilities.IsRight(enteranceIndex))
            {
                dirFromEntrance = new Vector2Int(-1, 0);
            }
            else if (GridUtilities.IsTop(enteranceIndex))
            {
                dirFromEntrance = new Vector2Int(0, -1);
            }
            else if (!GridUtilities.IsBottom(enteranceIndex))
            {
                Debug.Log("Enterance must be on edge of board");
            }

            startSquare = GridUtilities.GetNextSquare(startSquare, dirFromEntrance);

            if (startSquare.hasBox)
            {
                Debug.LogWarning("Enterance not valid");
                continue;
            }

            Dictionary<int, Junction> currentPath = new Dictionary<int, Junction>();

            //Loop through exits
            foreach (int exitIndex in Exits)
            {
                MaxPath = (Grid.width / 2) * (Grid.height / 2) + ((Grid.width + Grid.height) /4);
                LowestTotalPath = MaxPath;


            }
        }
    }

}
