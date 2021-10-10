using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    //level progress
    //level location -->done
    //level objective -->done
    //misc level data
    [SerializeField] private string location;
    [SerializeField] private string objective;

    public string Location => location;
    public string Objective => objective;

    public void SetLocation(string newLocation)
    {
        location = newLocation;
    }

    public void SetObjective(string newObjective)
    {
        objective = newObjective;
    }
}
