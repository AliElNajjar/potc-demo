using System;
using UnityEngine;

//    -Luck: This determines the unit’s ability to land and to dodge critical hits.
//        The necessary critical skill must be learned before critical hits can be used.
//-Charisma(Cha) : The unit’s willpower.Influences damage with and resistance against certain mind-based attacks.
//    Also affects effectiveness of a unit’s partner units and team attacks.
//-Deftness (Deft): Magical evasion. This is subtracted from the opponent’s Acuity stat.

[Serializable]
public class Spirit : ParameterBase
{
    [SerializeField] private float luckPower;
    [SerializeField] private float deftnessPower;
    [SerializeField] private float charismaPower;
    
    public float LuckPower => luckPower;
    public float DeftnessPower => deftnessPower;
    public float CharismaPower => charismaPower;

    protected override void CalculateStats(float[] multipliers)
    {
        luckPower = baseValue * multipliers[0];
        deftnessPower = baseValue * multipliers[1];
        charismaPower = baseValue * multipliers[2];
    }
}