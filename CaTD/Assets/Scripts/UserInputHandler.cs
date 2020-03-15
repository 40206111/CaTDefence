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

    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Square.DrawBoxes();
        }
    }
}
