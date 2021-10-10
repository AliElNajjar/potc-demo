using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMovement : MonoBehaviour
{
    //free movement controls
    //squares, each square = 10 units of movement
    //after move => target => battle info => confirm => battle.
    
    [SerializeField] private float movementValue = 0.5f;
    private int numberOfSteps;

    private float AllowedMovement()
    {
        return numberOfSteps * movementValue;
    }
}
