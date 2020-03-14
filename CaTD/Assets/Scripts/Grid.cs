using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Transform cameraTrans;
    [SerializeField] GameObject aBox;
    public int width;
    public int height;

    class Box
    {
        public GameObject box;
        public bool hasBox => box != null;
        public Vector3 pos;

        public Box(Vector3 newPos)
        {
            pos = newPos;
            box = null;
        }
    }

    List<Box> boxes = new List<Box>();

    // Start is called before the first frame update
    void Awake()
    {
        GenerateGrid();
        RandomBoxes();

        Vector3 camPos = boxes[0].pos + boxes[boxes.Count - 1].pos;
        camPos /= 2;
        camPos.z = -10;
        cameraTrans.position = camPos;
    }

    void GenerateGrid()
    {
        Vector2 start = new Vector2(0, 0);
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                boxes.Add(new Box(new Vector3(start.x + j, start.y + (0.5f * j), (0.5f * j) + i)));
            }
            start.x -= 1;
            start.y += 0.5f;
        }
    }

    void RandomBoxes()
    {
        for (int i = 0; i < boxes.Count; ++i)
        {
            int r = Random.Range(0, 2);
            if (r == 1)
            {
                aBox.transform.position = boxes[i].pos;
                boxes[i].box = Instantiate(aBox, transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Box point in boxes)
        {
            Gizmos.DrawSphere(point.pos, 0.2f);
        }
    }
}
