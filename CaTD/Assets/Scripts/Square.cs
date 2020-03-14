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
        hasBox = true;
    }

    public void RemoveBox()
    {
        sr.sprite = squareSprite;
        hasBox = false;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        sr.material.color = Color.green;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !hasBox)
        {
            AddBox();
        }
        else if (Input.GetMouseButtonDown(1) && hasBox)
        {
            RemoveBox();
        }
    }

    void OnMouseExit()
    {
        sr.material.color = Color.white;
    }
}
