using System;
using UnityEngine;

//    -Mana Points(MP): These are required to cast magic spells or use active skills.
//        Mana can be regenerated via items or by standing next to a crystal when available.When below 25%,
//        units become deficient, reducing combat abilities(perhaps by halving their main parameters?).
//        If all MP is used, the unit becomes deprived,
//        and cannot use their bonus action or reaction, on top of becoming weaker still.

//-Resilience (Res): This determines how well the unit can withstand magical attacks, and affects recovery spells.

[Serializable]
public class Wisdom : ParameterBase
{
    [SerializeField] private float manaPoints;
    [SerializeField] private float magicResist;
    
    public float ManaPoints => manaPoints;

    public float MagicResist => magicResist;

    public void ChangeManaPoint(float value)
    {
        manaPoints += value;
    }
    
    protected override void CalculateStats(float[] multipliers)
    {
        manaPoints  = baseValue * multipliers[0];
        magicResist = baseValue * multipliers[1];
    }
}