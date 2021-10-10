using System;
using UnityEngine;

[Serializable]
public abstract class Item : ScriptableObject
{
    [SerializeField] protected BaseItemType baseType;

    [SerializeField] protected int numOfUse;
    [SerializeField] protected float buyValue;
    [SerializeField] protected float sellValue;
    [SerializeField] protected string text;

    public BaseItemType BaseType { get => baseType; set => baseType = value; }
    public int NumOfUse { get => numOfUse; set => numOfUse = value; }
    public float BuyValue { get => buyValue; set => buyValue = value; }
    public float SellValue { get => sellValue; set => sellValue = value; }
    public string Text { get => text; set => text = value; }
    
    public abstract void Use();
    
    
    public enum BaseItemType
    {
        Scroll = 0, Booster = 1, Material = 2
    }
}