using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ParameterBase
{
    [SerializeField] protected float baseValue;
    
    public float BaseValue => baseValue;
    
    protected abstract void CalculateStats(float[] multipliers);
    
    public void AffectStats(float[] multipliers)
    {
        CalculateStats(multipliers);
    }

    public void SetBaseValue(float baseStat)
    {
        baseValue = baseStat;
    }
}

public class ParameterMultiplier : ScriptableObject
{
    
}