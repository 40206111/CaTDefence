using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    [SerializeField] float minSize;
    [SerializeField] float maxSize;
    [SerializeField] float zoomSpeed = 150f;
    [SerializeField] float moveSpeed = 4.0f;
    [SerializeField][Range(0, 1)] float edgeLenience = 0.95f;
    public int invertY = -1;
    Camera cam;
    List<Vector2> bounds = new List<Vector2>();

    static public bool Move = true;

    public void CenterOnGrid()
    {
        Vector2 bottom = Grid.squares[0].pos;
        Vector2 top = Grid.squares[Grid.squares.Count - 1].pos;
        Vector2 left = Grid.squares[Grid.squares.Count - Grid.width].pos;
        Vector2 right = Grid.squares[Grid.width - 1].pos;
        bounds.Add(bottom);
        bounds.Add(left);
        bounds.Add(top);
        bounds.Add(right);

        Vector3 camPos = bottom + top;
        camPos /= 2;
        camPos.z = -10;
        transform.position = camPos;
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void getClosest2Bounds(out Vector2 bound1, out Vector2 bound2)
    {
        Vector2 pos = transform.position;
        bound1 = bounds[0];
        bound2 = bound1;
        float closest = (bounds[0] - pos).sqrMagnitude;
        float secondClosest = closest;

        for (int i = 1; i < bounds.Count; ++i)
        {
            float sqrDist = (bounds[i] - pos).sqrMagnitude;
            if (sqrDist < closest)
            {
                bound2 = bound1;
                bound1 = bounds[i];
                secondClosest = closest;
                closest = sqrDist;
            }
            else if (sqrDist < secondClosest)
            {
                bound2 = bounds[i];
                secondClosest = sqrDist;
            }
        }

    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 thisPos = transform.position;

        //Zoom
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        float camSize = cam.orthographicSize;
        float newCamSize = camSize + (scrollValue * zoomSpeed * Time.deltaTime * invertY);
        cam.orthographicSize = Mathf.Clamp(newCamSize, minSize, maxSize);

        if (!Move)
        {
            return;
        }

        //Edge detection
        if (mousePos.y > Screen.height * edgeLenience)
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
        }
        else if (mousePos.y < Screen.height * (1- edgeLenience))
        {
            transform.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
        }
        if (mousePos.x > Screen.width * edgeLenience)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
        }
        else if (mousePos.x < Screen.width * (1 - edgeLenience))
        {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
        }

    }
}
