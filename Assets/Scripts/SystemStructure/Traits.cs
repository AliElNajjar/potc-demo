using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenusTrait", menuName = "Unit/Create Traits", order = 1)]
public class Traits : ScriptableObject
{
    [SerializeField] private UnitParameters traitsParams;
    [SerializeField] private List<Effect> baseBuffs;
    [SerializeField] private List<Effect> baseDebuff;
    [SerializeField] private List<Skill> baseSkill;
}