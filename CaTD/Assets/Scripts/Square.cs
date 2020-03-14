using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class Square : MonoBehaviour
{
    [SerializeField] Sprite squareSprite;
    [SerializeField] Sprite boxSprite;

    SpriteRenderer sr;

    public bool hasBox = false;
    public Vector3 pos;

    public void CreateSquare(Vector3 newPos)
    {
        pos = newPos;
        Vector3 modifiedPos = pos;
        modifiedPos.y -= 0.5f;
        transform.position = modifiedPos;
        sr.sprite = squareSprite;
    }

    public void AddBox()
    {
        sr.sprite = boxSprite;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
}
