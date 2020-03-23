using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCNode
{
    static int factionCount = 0;
    static List<int> freeFactions = new List<int>();
    static List<int> keyFactions = new List<int>();

    private int distance = -1;
    public int Distance
    {
        get { return distance; }
        set
        {
            distance = value;
            PathChecker.Instance.UpdateDistanceText(id);
        }
    }

    public int distSource = -1;
    public int faction = -1; // 0 is floor
    public int id = -1;
    public Vector2Int position;
    public List<PCNode> neighbours = new List<PCNode>();
    public List<PCNode> lowers = new List<PCNode>();
    public List<PCNode> uppers = new List<PCNode>();

    public PCNode(int id, Vector2Int pos)
    {
        faction = 0;
        this.id = id;
        position = pos;
    }

    public void AddNeighbours(List<PCNode> ns)
    {
        neighbours = ns;
    }

    // Returns faction box became
    public int MakeBox(ref List<int> nodeQueue)
    {
        // Find the faction to join
        int lowest = int.MaxValue;
        foreach (PCNode n in neighbours)
        {
            if (n.faction > 0)
            {
                lowest = Mathf.Min(lowest, n.faction);
            }
        }
        // If a faction was found: join it, and list neighbours who aren't a member
        if (lowest != int.MaxValue)
        {
            foreach (PCNode n in neighbours)
            {
                if (n.faction > 0 && n.faction != lowest)
                {
                    nodeQueue.Add(n.id);
                }
            }
            faction = lowest;
            Distance = -1;
        }
        // If no faction found, make one
        else
        {
            // Join an extinct faction if any exist
            if (freeFactions.Count > 0)
            {
                faction = freeFactions[0];
                freeFactions.RemoveAt(0);
            }
            // Create a brand new faction
            else
            {
                ++factionCount;
                faction = factionCount;
            }
            Distance = 0;
        }
        return faction;
    }

    public void ChangeFaction(ref List<int> nodeQueue, int newF)
    {
        int oldF = faction;
        faction = newF;
        Distance = -1;
        foreach (PCNode n in neighbours)
        {
            if (n.faction == oldF)
            {
                if (!nodeQueue.Contains(n.id))
                {
                    nodeQueue.Add(n.id);
                }
            }
        }
    }

    public void CalculateDistance(ref List<int> nodeQueue)
    {
        // If node has a distance, leave
        if (Distance != -1)
        {
            return;
        }

        int lowest = int.MaxValue;
        foreach (PCNode n in neighbours)
        {
            if (n.faction == faction && n.Distance != -1)
            {
                lowest = Mathf.Min(lowest, n.Distance);
            }
        }

        // If no neighbours exist, leave
        if (lowest == int.MaxValue)
        {
            return;
        }

        Distance = lowest + 1;
        foreach (PCNode n in neighbours)
        {
            if (n.faction == faction)
            {
                if (n.Distance == lowest)
                {
                    if (!uppers.Contains(n))
                    {
                        uppers.Add(n);
                        n.AdoptLower(this);
                    }
                }
                else if(n.Distance > Distance + 1)
                {
                    nodeQueue.Add(n.id);
                }
            }
        }
    }

    public void CalculateDistanceCascade(ref List<int> nodeQueue)
    {
        CalculateDistance(ref nodeQueue);
        foreach (PCNode n in neighbours)
        {
            if (n.faction == faction && n.Distance == -1 && !nodeQueue.Contains(n.id))
            {
                nodeQueue.Add(n.id);
            }
        }
    }

    public void AdoptLower(PCNode l)
    {
        if (!lowers.Contains(l))
        {
            lowers.Add(l);
        }
    }

    public void AddUpper(PCNode u)
    {
        if (!uppers.Contains(u))
        {
            uppers.Add(u);
        }
    }

    public void RemoveLower(PCNode l)
    {
        if (lowers.Contains(l))
        {
            lowers.Remove(l);
        }
    }

    public void RemoveUpper(PCNode u)
    {
        if (uppers.Contains(u))
        {
            uppers.Remove(u);
        }
    }

    public void UnmakeBox(ref List<int> nodeQueue)
    {
        foreach (PCNode n in lowers)
        {
            if (!nodeQueue.Contains(n.id))
            {
                nodeQueue.Add(n.id);
            }
        }
    }

    public int SearchForParents(ref List<int> nodeQueue)
    {
        // If parents, do not wipe distance, end search
        if (uppers.Count > 0)
        {
            return 1;
        }

        // If no parents wipe distance
        Distance = -1;
        // If no children, exit
        if (lowers.Count == 0)
        {
            return -1;
        }

        // If children add to queue for parent searching
        foreach (PCNode n in lowers)
        {
            n.RemoveUpper(this);
            if (!nodeQueue.Contains(n.id))
            {
                nodeQueue.Add(n.id);
            }
        }

        return 0;
    }

    public void NearbyOrphans(ref List<int> nodeQueue)
    {
        foreach (PCNode n in neighbours)
        {
            if (n.faction == faction && n.Distance == -1 && !nodeQueue.Contains(n.id))
            {
                nodeQueue.Add(n.id);
            }
        }
    }

    public void DistanceFix(ref List<int> nodeQueue)
    {
        int lowest = int.MaxValue;

        foreach (PCNode n in neighbours)
        {
            if (n.faction == faction && n.Distance != -1)
            {
                lowest = Mathf.Min(n.Distance, lowest);
            }
        }

        // Set own distance
        if (lowest == int.MaxValue)
        {
            Distance = 0;
        }
        else
        {
            Distance = lowest + 1;
        }
        // Clear uppers and remove self as lower from them
        foreach (PCNode n in uppers)
        {
            n.RemoveLower(this);
        }
        uppers.Clear();
        // Clear lowers and remove self as upper
        foreach (PCNode n in lowers)
        {
            n.RemoveUpper(this);
        }
        lowers.Clear();
        // Locate new uppers/lowers
        foreach (PCNode n in neighbours)
        {
            if (n.faction == faction)
            {
                if (n.Distance == -1)
                {
                    continue;
                }
                else if (n.Distance < Distance)
                {
                    uppers.Add(n);
                    n.AdoptLower(this);
                }
                else if (n.Distance > Distance)
                {
                    lowers.Add(n);
                    n.AddUpper(this);
                    if (n.Distance > Distance + 1 && !nodeQueue.Contains(n.id))
                    {
                        nodeQueue.Add(n.id);
                    }
                }
            }
        }
    }
}

public class PathChecker : MonoBehaviour
{
    #region singleton
    static private PathChecker instance;
    static public PathChecker Instance
    {
        get { return instance; }
    }
    #endregion

    private int outerGroups = 0;

    private List<PCNode> nodes = new List<PCNode>();
    private List<Text> nodeDistances = new List<Text>();

    private List<Vector2Int> neighbourDirections = new List<Vector2Int> {
        Vector2Int.up, Vector2Int.up + Vector2Int.right, Vector2Int.right, Vector2Int.right + Vector2Int.down,
        Vector2Int.down, Vector2Int.down + Vector2Int.left, Vector2Int.left, Vector2Int.left + Vector2Int.up
    };

    private bool ValidNeighbour(Vector2Int pos)
    {
        return !(pos.x < 0 || pos.x > Grid.width - 1 || pos.y < 0 || pos.y > Grid.height - 1);
    }

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        int length = Grid.width * Grid.height;
        for (int i = 0; i < length; ++i)
        {
            nodes.Add(new PCNode(i, GridUtilities.OneToTwo(i)));
        }
        foreach (PCNode pcn in nodes)
        {
            List<PCNode> neighbours = new List<PCNode>();
            foreach (Vector2Int v in neighbourDirections)
            {
                Vector2Int newLoc = pcn.position + v;
                if (ValidNeighbour(newLoc))
                {
                    neighbours.Add(nodes[GridUtilities.TwoToOne(newLoc)]);
                }
            }
            pcn.AddNeighbours(neighbours);
        }
        foreach (Square s in Grid.squares)
        {
            nodeDistances.Add(s.GetComponentInChildren<Text>());
        }
    }

    public void AddBox(int index)
    {
        AddBox(GridUtilities.OneToTwo(index));
    }
    public void AddBox(Vector2Int pos)
    {
        List<int> recruits = new List<int> { GridUtilities.TwoToOne(pos) };
        // Turn position into a box ~~~ if not already a box
        int faction = nodes[GridUtilities.TwoToOne(pos)].MakeBox(ref recruits);
        // Turn any contacts to its side
        for (int i = 1; i < recruits.Count; ++i)
        {
            nodes[recruits[i]].ChangeFaction(ref recruits, faction);
        }
        // Calculate distances
        List<int> miscounted = new List<int>();
        foreach (int i in recruits)
        {
            nodes[i].CalculateDistance(ref miscounted);
            for (int j = 0; j < miscounted.Count; ++j)
            {
                nodes[miscounted[j]].DistanceFix(ref miscounted);
            }
        }
    }

    public void RemoveBox(Vector2Int pos)
    {
        List<int> orphans = new List<int>();
        nodes[GridUtilities.TwoToOne(pos)].UnmakeBox(ref orphans);
        List<int> origins = new List<int>();
        for (int i = 0; i < orphans.Count; ++i)
        {
            if (nodes[orphans[i]].SearchForParents(ref orphans) == 1)
            {
                origins.Add(orphans[i]);
            }
        }

        orphans.Clear();
        foreach (int i in origins)
        {
            nodes[i].NearbyOrphans(ref orphans);
        }

        for (int i = 0; i < orphans.Count; ++i)
        {
            nodes[orphans[i]].CalculateDistanceCascade(ref orphans);
        }
    }

    public void UpdateDistanceText(int index)
    {
        nodeDistances[index].text = nodes[index].Distance.ToString();
        nodeDistances[index].color = FancyColour(nodes[index].Distance);
    }

    private Color FancyColour(int distance)
    {
        Color returnColour = Color.black;
        float loopCap = 50.0f;
        returnColour.r = Mathf.Sin(Mathf.PI * 2.0f * distance / loopCap) * 0.5f + 0.5f;
        returnColour.g = Mathf.Sin(Mathf.PI * 2.0f * (distance + (loopCap/3.0f)) / loopCap) * 0.5f + 0.5f;
        returnColour.b = Mathf.Sin(Mathf.PI * 2.0f * (distance + (2.0f* loopCap/3.0f)) / loopCap) * 0.5f + 0.5f;
        return returnColour;
    }
}
