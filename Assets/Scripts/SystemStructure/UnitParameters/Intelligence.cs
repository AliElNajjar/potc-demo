using System;
using UnityEngine;

//-Magic(Mag) :The amount of damage the unit can deal with magic attacks and spells.
//        The value is checked against the opponent’s res stat(s). 
//-Acuity(Acty) : Magical accuracy.
//        The chances the unit will land a hit with a magical attack or spell.
//Calculated using the unit’s Int added to their spell’s acuity.

[Serializable]
public class Intelligence : ParameterBase
{
    [SerializeField] private float magicPower;
    [SerializeField] private float acuityPower;
    
    public float MagicPower => magicPower;

    public float AcuityPower => acuityPower;

    protected override void CalculateStats(float[] multipliers)
    {
        magicPower = baseValue * multipliers[0];
        acuityPower = baseValue * multipliers[1];
    }
}