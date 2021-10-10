using System;
using UnityEngine;

[Serializable]
public struct UnitParameters
{
    [SerializeField] private string unitName;
    [SerializeField] private string unitDescription;
    [SerializeField] private Endurance enduranceParam;
    [SerializeField] private Strength strengthParam;
    [SerializeField] private Intelligence intelligenceParam;
    [SerializeField] private Dexterity agilityParam;
    [SerializeField] private Wisdom wisdomParam;
    [SerializeField] private Spirit spiritParam;

    public enum Stats
    {
        Endurance = 0,
        Strength = 1,
        Intelligence = 2,
        Agility = 3,
        Wisdom = 4,
        Spirit = 5
    }
    
    public Endurance EnduranceParam => enduranceParam;
    public Strength StrengthParam => strengthParam;
    public Intelligence IntelligenceParam => intelligenceParam;
    public Dexterity AgilityParam => agilityParam;
    public Wisdom WisdomParam => wisdomParam;
    public Spirit SpiritParam => spiritParam;

    public string UnitName => unitName;
    public string UnitDescription => unitDescription;


    public ParameterBase this[Stats index]
    {
        get
        {
            switch (index)
            {
                case Stats.Endurance:
                    return EnduranceParam;
                case Stats.Strength:
                    return StrengthParam;
                case Stats.Intelligence:
                    return IntelligenceParam;
                case Stats.Agility:
                    return AgilityParam;
                case Stats.Wisdom:
                    return WisdomParam;
                case Stats.Spirit:
                    return SpiritParam;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }
}