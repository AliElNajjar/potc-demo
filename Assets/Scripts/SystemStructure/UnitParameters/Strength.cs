using System;
using UnityEngine;

//-Attack(ATK) : The amount of damage the unit can deal with physical attacks and skills.
//        This value is checked against the opponent’s def stat(s).
//-Defense(Def) : The unit’s ability to withstand physical attacks and skills.

[Serializable]
public class Strength : ParameterBase
{
    [SerializeField] private float attackPower;
    [SerializeField] private float defensePower;
    
    public float AttackPower => attackPower;

    public float DefensePower => defensePower;

    protected override void CalculateStats(float[] multipliers)
    {
        attackPower = baseValue * multipliers[0];
        defensePower = baseValue * multipliers[1];
    }
}