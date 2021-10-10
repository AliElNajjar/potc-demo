using System;
using UnityEngine;

//    -Initiative(Init) : Turn order.The unit with the highest value goes first, 
// and the unit with the lowest goes last. Any changes in initiative don’t go into effect until the following round.

//-Accuracy (Acc): Physical accuracy. 
// The chances the unit will land a hit with a physical attack or skill.
//-Evasion (Evsn): Physical evasion. 
//                 The chances the unit will dodge a physical attack or skill.

[Serializable]
public class Dexterity : ParameterBase
{
    [SerializeField] private float initiativePower;
    [SerializeField] private float accuracyPower;
    [SerializeField] private float evasionPower;

    public float InitiativePower => initiativePower;
    
    public float AccuracyPower => accuracyPower;
    
    public float EvasionPower => evasionPower;


    protected override void CalculateStats(float[] multipliers)
    {
        initiativePower = baseValue * multipliers[0];
        accuracyPower = baseValue * multipliers[1];
        evasionPower = baseValue * multipliers[2];
    }
}