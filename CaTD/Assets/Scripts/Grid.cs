using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject aBox;
    [SerializeField] CameraControl cameraControl;

    static public int width = 7;
    static public int height = 8;

    static public Square TopCorner;
    static public Square BottomCorner;
    static public Square LeftCorner;
    static public Square RightCorner;

    static public int enterance = 0;
    static public int exit = 0;
    static public List<Vector2Int> enterances;
    static public List<Vector2Int> exits;

    public static List<Square> squares = new List<Square>();

    void Awake()
    {
        GenerateGrid(width, height);
        cameraControl.CenterOnGrid();

        enterances = new List<Vector2Int>();
        for (int i = 1; i < Grid.width - 1; i++)
        {
            enterances.Add(Grid.squares[i].gridCoord);
            Grid.squares[i].RemoveBox(force: true);
        }

        exits = new List<Vector2Int>();
        for (int i = (Grid.squares.Count - Grid.width) + 1; i < Grid.squares.Count - 1; i++)
        {
            exits.Add(Grid.squares[i].gridCoord);
            Grid.squares[i].RemoveBox(force: true);
        }
    }

    //This method will need to be called at the start of a level to generate the correct size of grid
    void GenerateGrid(int w, int h)
    {
        //store width and height for easy access
        width = w;
        height = h;
        Vector2 start = new Vector2(0, 0);
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                squares.Add(Instantiate(aBox, transform).GetComponent<Square>());
                squares[squares.Count - 1].CreateSquare(new Vector3(start.x + j, start.y + (0.5f * j), (0.5f * j) + i), 
                                                        new Vector2Int(j, i));
                if (i == 0 || i == height - 1 || j == 0 || j == width - 1)
                {
                    squares[squares.Count - 1].AddBox(force: true);
                    squares[squares.Count - 1].edge = true;
                }
            }
            start.x -= 1;
            start.y += 0.5f;
        }
    }

    //Debug method to populate with random boxes to test map
    //TODO: change to stick to grid rules to allow for pathing testing
    void RandomBoxes()
    {
        for (int i = 0; i < squares.Count; ++i)
        {
            int r = Random.Range(0, 2);
            if (r == 1)
            {
                squares[i].AddBox();
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //foreach (Square point in squares)
        //{
        //    Gizmos.DrawSphere(point.pos, 0.2f);
        //}

        if (AiPathing.MasterPath == null) return;

        Gizmos.color = Color.red;

        int enteranceKey = GridUtilities.TwoToOne(enterances[enterance]);

        if (!AiPathing.MasterPath.ContainsKey(enteranceKey)) return;

        var enterancePaths = AiPathing.MasterPath[enteranceKey];

        int key = GridUtilities.TwoToOne(exits[exit]);

        if (!enterancePaths.ContainsKey(key)) return;

        var exitPaths = enterancePaths[key];

        DrawPath(exitPaths);
    }

    void DrawPath(Path path)
    {
        if (path == null) return;
        if (path.FutureSquares == null) return;

        foreach (Path p in path.FutureSquares)
        {
            DrawPath(path.Current, p);
        }
    }

    void DrawPath(Square before, Path now)
    {
        Gizmos.DrawLine(before.pos, now.Current.pos);

        if (now.FutureSquares == null) return;

        foreach(Path path in now.FutureSquares)
        {
            DrawPath(now.Current, path);
        }
    }

}
