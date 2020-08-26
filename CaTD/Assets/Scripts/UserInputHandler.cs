using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class was created to help the squares draw boxes even if the click finishes
/// outside of the box grid.
/// 
/// It seems reasonable to use this for any similar user input checks we may have in future.
/// </summary>
public class UserInputHandler : MonoBehaviour
{
    [SerializeField] CameraControl camControl;


    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Square.DrawBoxes();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Square.highlightColour = Color.yellow;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Square.highlightColour = Color.red;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            camControl.CenterOnGrid();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Grid.enterance + 1 >= Grid.enterances.Count)
            {
                Grid.enterance = 0;
            }
            else
            {
                Grid.enterance++;
            }
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Grid.exit + 1 >= Grid.exits.Count)
            {
                Grid.exit = 0;
            }
            else
            {
                Grid.exit++;
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Grid.Pathing.SetEnterancesAndExits(Grid.enterances, Grid.exits);
            StartCoroutine(Grid.Pathing.CalculatePath());
        }
#endif  


        if (Input.GetKeyDown(KeyCode.P))
        {
            CameraControl.Move = !CameraControl.Move;
        }

    }
}
