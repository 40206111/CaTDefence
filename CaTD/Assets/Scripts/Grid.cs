using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject aBox;
    [SerializeField] CameraControl cameraControl;

    static public int width = 13;
    static public int height = 20;

    public static List<Square> squares = new List<Square>();

    // Start is called before the first frame update
    void Awake()
    {
        GenerateGrid();
        cameraControl.CenterOnGrid();
    }

    void GenerateGrid()
    {
        Vector2 start = new Vector2(0, 0);
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                squares.Add(Instantiate(aBox, transform).GetComponent<Square>());
                squares[squares.Count - 1].CreateSquare(new Vector3(start.x + j, start.y + (0.5f * j), (0.5f * j) + i));
            }
            start.x -= 1;
            start.y += 0.5f;
        }
    }

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
        Gizmos.color = Color.green;
        foreach (Square point in squares)
        {
            Gizmos.DrawSphere(point.pos, 0.2f);
        }
    }
}
