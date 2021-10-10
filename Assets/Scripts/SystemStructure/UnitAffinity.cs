using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Elements and affinities are essentially the same thing.
//Every unit has one affinity, one of the eight elements,
//which determines what element they are weak against, and what they are strong against.

//Their element resistances use their affinity as a baseline, but can be altered by their equipment, 
//to patch up a weakness or to resist against other elements. The name isn't particularly important,
//it just functions as quick information for the player. They'll know if they see an enemy with a fire affinity, water will be effective against it.

//Limit is a common term used in RPGs to represent a sort of "finishing move". 
//They are more powerful than most skills, but generally require some kind of setup to use.
//Final Fantasy 7 and up are examples of RPGs that use these.

public class UnitAffinity : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
