using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitClass", menuName = "Unit/Create Unit Class", order = 1)]
public partial class UnitClass : ScriptableObject
{
    [SerializeField] private string className;
    [SerializeField] private string classDescription;
    [SerializeField] private List<Multiplier> multipliers;
    [SerializeField] private UnitResistances resistances;
    [SerializeField] private float movementValue;
    
    [Serializable]
    public class Multiplier : SerializableDictionary<UnitParameters.Stats, float[]>
    {
    }

    public UnitResistances Resistances => resistances;

    public UnitParameters Apply(UnitParameters parameters, UnitResistances resistances)
    {
        for (int i = 0; i < Enum.GetValues((typeof(UnitParameters.Stats))).Length; i++)
        {
            parameters[(UnitParameters.Stats) i].AffectStats(multipliers.Find(x=> x.key == (UnitParameters.Stats)i).value);
        }

        return parameters;
    }
}