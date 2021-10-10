using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Unit/Create Armor", order = 1)]
public class Armor : ScriptableObject, ISellable, IWearable
{
    [SerializeField] private UnitParameters parameters;
    [SerializeField] private ArmorType type;
    [SerializeField] private BodyPart part;
    [SerializeField] private float buyValue;
    [SerializeField] private float sellValue;
    [SerializeField] private float level;
    [SerializeField] private float weight;

    public float BuyValue { get => buyValue; set => buyValue = value; }

    public float SellValue { get => sellValue; set => sellValue = value; }

    public float Level { get => level; set => level = value; }


    public float Weight { get => weight; set => weight = value; }

    public UnitParameters Parameters => parameters;
}

public enum ArmorType
{
    None = 0,
    Light = 1,
    Medium = 2,
    Heavy = 3,
    Arcane = 4
}

public enum BodyPart
{
    Head = 0,
    Body = 1,
    Gloves = 2,
    Legs = 3,
    Boots = 4
}