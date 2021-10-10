using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create Unit", order = 1)]
public class Unit : ScriptableObject
{
    [SerializeField] private UnitImages unitImages; 
    [SerializeField] private UnitParameters unitParams;
    [SerializeField] private UnitClass unitClass;
    [SerializeField] private List<Effect> baseBuffs;
    [SerializeField] private List<Armor> armors;
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private List<Skill> baseSkill;
    [SerializeField] private List<Skill> earnedSkill;
    [SerializeField] private Traits traits;
    [SerializeField] private MoveType moveType;
    [SerializeField] private ShotsType shotType;
    [SerializeField] private Affinity affinity;
    [SerializeField] private RankType rankType;
    //[SerializeField] private int[] ItemSlots = new int[9];
    [SerializeField] private int parameterGrowth;
    [SerializeField] private int skillPointGrowth;
    //[SerializeField] private int[] Levels = new int[30];
    [SerializeField] private float experience;
    private float _resistance;

    public UnitClass UnitClass => unitClass;

    public UnitParameters UnitParams => unitParams;

    public UnitImages UnitImages => unitImages;

    public List<Effect> BaseBuffs => baseBuffs;

    public List<Weapon> Weapons => weapons;

    public List<Armor> Armors => armors;

    private void OnValidate()
    {
        unitParams = unitClass != null ? unitClass.Apply(UnitParams, null) : default;
    }
    
    
    #region UnitEnums

    public enum RankType
    {
        Base = 0,
        Advanced = 1,
        Special = 2
    }

    public enum Affinity
    {
        Light = 0,
        Wind = 1,
        Ice = 2,
        Water = 3,
        Earth = 4,
        Fire = 5,
        Thunder = 6,
        Dark = 7
    }

    public enum GenusTrait
    {
        Human = 0,
        Dyvan = 1,
        Altair = 2,
        Ferrough = 3,
        Dryad = 4,
        Eidolon = 5,
        Devil = 6,
        Material = 7
    }

    public enum Trait
    {
        Manaless = 0,
        Sensitive = 1,
        Flying = 2,
        Undead = 3,
        Armored = 4,
        None = 5
    }

    public enum ShotsType
    {
        Portrait = 0,
        HeadShot = 1,
        TowerShot = 2
    }

    public enum MoveType
    {
        Walk = 0,
        Fly = 1,
        Swim = 2,
        Hover = 3
    }

    #endregion
}