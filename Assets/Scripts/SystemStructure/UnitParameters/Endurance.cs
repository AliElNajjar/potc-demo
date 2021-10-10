using System;
using UnityEngine;

//-Hit Points(HP): When this number reaches zero, the unit is knocked unconscious. 
//-Constitution (Con): Loadout.If the unit’s equipment exceeds their total Con,
//they receive penalties to their ability to accuracy/acuity (hit) and evasion/deftness (evade) in combat.

[Serializable]
public class Endurance : ParameterBase
{
    [SerializeField] private float healthPoints;
    [SerializeField] private float constitutionPower;

    public float HealthPoints => healthPoints;

    public float ConstitutionPower => constitutionPower;

    public void ChangeHealthPoints(float value)
    {
        healthPoints += value;
    }

    protected override void CalculateStats(float[] multipliers)
    {
        healthPoints = baseValue * multipliers[0];
        constitutionPower = baseValue * multipliers[1];
    }
}