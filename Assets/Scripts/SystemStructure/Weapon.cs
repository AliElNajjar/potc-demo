using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Unit/Create Weapon", order = 1)]
public class Weapon : ScriptableObject, ISellable, IWearable
{
    [SerializeField] private UnitParameters stats;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private PhysicUnit physicalType;
    [SerializeField] private ElementalUnit elementalType;
    [SerializeField] private float buyValue;
    [SerializeField] private float sellValue;
    [SerializeField] private float level;
    [SerializeField] private WeaponSize size;
    [SerializeField] private float weight;

    public float BuyValue { get => buyValue; set => buyValue = value; }

    public float SellValue { get => sellValue; set => sellValue = value; }

    public float Level { get => level; set => level = value; }


    public float Weight { get => weight; set => weight = value; }

    public WeaponSize Size => size;
    
    public PhysicUnit PhysicalType => physicalType;

    public ElementalUnit ElementalType => elementalType;

    public UnitParameters Stats => stats;
}

public enum WeaponSize
{
    OneHanded,
    TwoHanded,
}

public enum WeaponType
{
    Sword = 0,
    Lance = 1,
    Axe = 2,
    Fists = 3,
    Shield = 4,
    Knives = 5,
    Bow = 6,
    Staff = 7,
    Tome = 8
}