using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Resistances", menuName = "Unit/Create Resistances", order = 1)]
[Serializable]
public class UnitResistances : ScriptableObject
{
    [SerializeField] private List<ElementalResistance> elementalResistance;
    [SerializeField] private List<PhysicResistance> physicResistance;

    public List<ElementalResistance> ElementResist => elementalResistance;

    public List<PhysicResistance> PhysicResist => physicResistance;
}
