using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    [SerializeField] float minSize;
    [SerializeField] float maxSize;
    [SerializeField] float speed = 150f;
    public int invertY = -1;
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        float camSize = cam.orthographicSize;
        float newCamSize = camSize + (scrollValue * speed * Time.deltaTime * invertY);
        cam.orthographicSize = Mathf.Clamp(newCamSize, minSize, maxSize);

    }
}
