using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Square : MonoBehaviour
{
    [SerializeField] Sprite squareSprite;
    [SerializeField] Sprite boxSprite;

    SpriteRenderer sr;

    enum eSquareState
    {
        none = 0,
        selected = 1 << 0,
        highlighted = 1 << 1,
    }

    eSquareState state;

    private bool hasBox = false;
    public bool HasBox
    {
        get { return hasBox; }
        set
        {
            hasBox = value;
            OnSquareChanged?.Invoke(gridCoord, hasBox);
        }
    }

    public bool edge = false;
    public Vector3 pos;
    public Vector2Int gridCoord;

    static Vector2Int startSquare;
    static Vector2Int endSquare;
    static List<int> drawPathIndexes = new List<int>();
    static bool drawHorizontal;
    public static Color highlightColour;
    static Color selectColour = Color.green;
    static Color resetColour;

    public delegate void SquareChanged(Vector2Int coord, bool boxAdded);
    public static SquareChanged OnSquareChanged;

    public void CreateSquare(Vector3 newPos, Vector2Int coord)
    {
        pos = newPos;
        Vector3 modifiedPos = newPos;
        modifiedPos.y -= 0.5f;
        transform.position = modifiedPos;
        sr.sprite = squareSprite;
        gridCoord = coord;
    }

    public bool AddBox(bool force = false)
    {
        if (HasBox || edge && !force)
        {
            return false;
        }
        sr.sprite = boxSprite;
        HasBox = true;
        return true;
    }

    public bool RemoveBox(bool force = false)
    {
        if (!HasBox || edge && !force)
        {
            return false;
        }
        sr.sprite = squareSprite;
        HasBox = false;
        return true;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        resetColour = sr.material.color;
    }

    private void LateUpdate()
    {
        if (edge) { return; }
        //set material colour based on state
        if (state.HasFlag(eSquareState.selected))
        {
            SetMaterialColour(selectColour);
        }
        else if (state.HasFlag(eSquareState.highlighted))
        {
            SetMaterialColour(highlightColour);
        }
        else if (state == eSquareState.none)
        {
            SetMaterialColour(resetColour);
        }
    }

    void SetMaterialColour(Color colour)
    {
        if (sr.material.color == colour)
        {
            return;
        }
        sr.material.color = colour;
    }

    void OnMouseEnter()
    {
        //tile is selected while mouse is over it.
        state |= eSquareState.selected;
    }

    public static void DrawBoxes()
    {
        foreach (int index in drawPathIndexes)
        {
            Grid.squares[index].state &= ~eSquareState.highlighted;
            if (Input.GetMouseButtonUp(0))
            {
                if (Grid.squares[index].AddBox())
                {
                    //PathChecker.Instance.AddBox(index);
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (Grid.squares[index].RemoveBox())
                {
                    //PathChecker.Instance.RemoveBox(index);
                }
            }
        }
    }

    private void RemovePathIndexes()
    {
        //remove highlighted state before clearing path indexes
        foreach (int index in drawPathIndexes)
        {
            Grid.squares[index].state &= ~eSquareState.highlighted;
        }
        drawPathIndexes.Clear();
    }

    string DrawPathIndexesString()
    {
        string output = "";
        foreach (int index in drawPathIndexes)
        {
            output += GridUtilities.OneToTwo(index).ToString();
            output += ", ";
        }
        return output;
    }

    private void OnMouseOver()
    {
        //if left or right click
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RemovePathIndexes();
            highlightColour = Input.GetMouseButtonDown(1) ? Color.red : Color.yellow; //red for delete, yellow for create
            state |= eSquareState.highlighted;
            startSquare = gridCoord;
            endSquare = startSquare;
            drawHorizontal = true;
            drawPathIndexes.Add(GridUtilities.TwoToOne(startSquare));
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) //left or right click down
        {
            //skip if same tile as last time
            if (gridCoord == endSquare)
            {
                return;
            }
            bool prevDrawHor = drawHorizontal;
            drawHorizontal = IsHorizontal(startSquare, gridCoord, drawHorizontal);

            //If we've changed direction our whole list is wrong
            if (prevDrawHor != drawHorizontal)
            {
                RemovePathIndexes();

                int startIndex = GridUtilities.TwoToOne(startSquare);
                int diff = drawHorizontal ? startSquare.x - gridCoord.x : startSquare.y - gridCoord.y;
                int direction = diff < 0 ? 1 : -1;
                int boxCount = Mathf.Abs(diff) + 1; //plus one to include starting box
                int modifier = drawHorizontal ? 1 : Grid.width;

                for (int i = 0; i < boxCount; ++i)
                {
                    int index = startIndex + (modifier * i * direction);
                    drawPathIndexes.Add(index);
                    Grid.squares[index].state |= eSquareState.highlighted;
                }
            }
            else
            {
                int diff = drawHorizontal ? startSquare.x - endSquare.x : startSquare.y - endSquare.y;
                int direction = diff < 0 ? 1 : -1;
                int newDiff = drawHorizontal ? endSquare.x - gridCoord.x : endSquare.y - gridCoord.y;
                int newDir = newDiff < 0 ? 1 : -1;
                int boxCount = Mathf.Abs(newDiff);
                int modifier = drawHorizontal ? 1 : Grid.width;

                for (int i = 0; i < boxCount; ++i)
                {
                    if (newDir != direction && diff != 0)
                    {
                        if (drawPathIndexes.Count == 1)
                        {
                            direction *= -1;
                            continue;
                        }
                        int index = drawPathIndexes[drawPathIndexes.Count - 1];
                        Grid.squares[index].state &= ~eSquareState.highlighted;
                        drawPathIndexes.Remove(index);
                    }
                    else
                    {
                        int index = drawPathIndexes[drawPathIndexes.Count - 1] + (modifier * newDir);
                        Grid.squares[index].state |= eSquareState.highlighted;
                        drawPathIndexes.Add(index);
                    }
                }
            }

            endSquare = gridCoord;
        }
    }

    public static bool IsHorizontal(Vector2Int coordOne, Vector2Int coordTwo, bool defaultReturn)
    {
        int xdif = Mathf.Abs(coordOne.x - coordTwo.x);
        int ydif = Mathf.Abs(coordOne.y - coordTwo.y);

        if (xdif == ydif)
        {
            return defaultReturn;
        }
        else if (xdif > ydif)
        {
            return true;
        }
        else if (ydif > xdif)
        {
            return false;
        }

        return defaultReturn;
    }

    void OnMouseExit()
    {
        //while mouse is over tile it is selected
        state &= ~eSquareState.selected;
    }
}
