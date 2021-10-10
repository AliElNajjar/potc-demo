using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [System.Serializable]
    public class StatModifier
    {

        #region Variables

        public Sprite icon;
        public string displayText;
        public Color textColor = Color.black;
        public bool hideInList;
        public string affectedStat;
        public EffectTypes effectType = EffectTypes.Instant;
        public EffectValueTarget valueTarget = EffectValueTarget.Value;
        public EffectValueTypes valueType = EffectValueTypes.Add;
        public string value = "0";

        internal string id = System.Guid.NewGuid().ToString();

        #endregion

        #region Properties

        public float AppliedValue { get; set; }

        public bool Initialized { get; set; }

        #endregion

        #region Public Methods

        public float CalculateAppliedValue(StatValue stat)
        {
            if (effectType != EffectTypes.Instant)
            {
                switch (valueTarget)
                {
                    case EffectValueTarget.MaximumValue:
                        return GetSustainedValue(stat.CurrentMaximum, stat) - stat.CurrentMaximum;
                    case EffectValueTarget.MinimumValue:
                        return GetSustainedValue(stat.CurrentMinimum, stat) - stat.CurrentMinimum;
                    case EffectValueTarget.RegenAmount:
                        return GetSustainedValue(stat.CurrentRegenAmount, stat) - stat.CurrentRegenAmount;
                    case EffectValueTarget.RegenDelay:
                        return GetSustainedValue(stat.CurrentRegenDelay, stat) - stat.CurrentRegenDelay;
                    case EffectValueTarget.Value:
                        if (valueType == EffectValueTypes.Add || valueType == EffectValueTypes.AddMultiplier)
                        {
                            return Mathf.Clamp(GetSustainedValue(stat.CurrentValue, stat) - stat.CurrentValue, stat.CurrentMinimum, stat.CurrentMaximum);
                        }
                        else
                        {
                            return GetSustainedValue(stat.CurrentValue, stat) - stat.CurrentValue;
                        }
                }
            }
            else
            {
                switch (valueTarget)
                {
                    case EffectValueTarget.MaximumValue:
                        return GetSustainedValue(stat.CurrentMaximum, stat);
                    case EffectValueTarget.MinimumValue:
                        return GetSustainedValue(stat.CurrentMinimum, stat);
                    case EffectValueTarget.RegenAmount:
                        return GetSustainedValue(stat.CurrentRegenAmount, stat);
                    case EffectValueTarget.RegenDelay:
                        return GetSustainedValue(stat.CurrentRegenDelay, stat);
                    case EffectValueTarget.Value:
                        if (valueType == EffectValueTypes.Add || valueType == EffectValueTypes.AddMultiplier)
                        {
                            return Mathf.Clamp(GetSustainedValue(stat.CurrentValue, stat) - stat.CurrentValue, stat.CurrentMinimum, stat.CurrentMaximum);
                        }
                        else
                        {
                            return GetSustainedValue(stat.CurrentValue, stat) - stat.CurrentValue;
                        }
                }
            }

            return 0;
        }

        public float CalculateAppliedValue(DamageModifier stat)
        {
            if (stat.modifierType == DamageModType.Resistance)
            {
                if (effectType != EffectTypes.Instant)
                {
                    switch (valueTarget)
                    {
                        case EffectValueTarget.Value:
                            return Mathf.Clamp(GetSustainedValue(stat.CurrentValue, stat), 0, 1) - stat.CurrentValue;
                        default:
                            return 0;
                    }
                }
                else
                {
                    switch (valueTarget)
                    {
                        case EffectValueTarget.Value:
                            return Mathf.Clamp(GetSustainedValue(stat.CurrentValue, stat), 0, 1);
                        default:
                            return 0;
                    }
                }
            }
            else
            {
                if (effectType != EffectTypes.Instant)
                {
                    switch (valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            return GetSustainedValue(stat.CurrentMaximum, stat) - stat.CurrentMaximum;
                        case EffectValueTarget.Value:
                            return Mathf.Clamp(GetSustainedValue(stat.CurrentValue, stat), 0, stat.CurrentMaximum) - stat.CurrentValue;
                    }
                }
                else
                {
                    switch (valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            return GetSustainedValue(stat.CurrentMaximum, stat);
                        case EffectValueTarget.Value:
                            return Mathf.Clamp(GetSustainedValue(stat.CurrentValue, stat), 0, stat.CurrentMaximum);
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// Creates a copy of the current object
        /// </summary>
        /// <returns></returns>
        public StatModifier Clone()
        {
            StatModifier sm = new StatModifier();

            sm.icon = icon;
            sm.displayText = displayText;
            sm.textColor = textColor;
            sm.hideInList = hideInList;
            sm.affectedStat = affectedStat;
            sm.effectType = effectType;
            sm.valueTarget = valueTarget;
            sm.valueType = valueType;
            sm.value = value;

            return sm;
        }

        /// <summary>
        /// Initialize modifier
        /// </summary>
        /// <param name="stat"></param>
        public void Initialize(StatValue stat)
        {
            AppliedValue = CalculateAppliedValue(stat);
            Initialized = true;
        }

        public void Initialize(DamageModifier modifier)
        {
            if (modifier.modifierType == DamageModType.Resistance)
            {
                AppliedValue = CalculateAppliedValue(modifier);
            }
            else
            {
                AppliedValue = CalculateAppliedValue(modifier);
            }
            Initialized = true;
        }

        /// <summary>
        /// Load modifer data from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            affectedStat = stream.ReadStringPacket();
            effectType = (EffectTypes)stream.ReadInt();
            valueTarget = (EffectValueTarget)stream.ReadInt();
            valueType = (EffectValueTypes)stream.ReadInt();
            value = stream.ReadStringPacket();
            AppliedValue = stream.ReadFloat();
            id = stream.ReadStringPacket();
            Initialized = true;
        }

        /// <summary>
        /// Save modifier data to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            stream.WriteStringPacket(affectedStat);
            stream.WriteInt((int)effectType);
            stream.WriteInt((int)valueTarget);
            stream.WriteInt((int)valueType);
            stream.WriteStringPacket(value);
            stream.WriteFloat(AppliedValue);
            stream.WriteStringPacket(id.ToString());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get unchanging value of modifier
        /// </summary>
        /// <param name="statVal"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        private float GetSustainedValue(float statVal, StatValue stat)
        {
            float modVal = stat.Parent.GetExpressionValue(value);
            switch (valueType)
            {
                case EffectValueTypes.AddMultiplier:
                    return statVal + (statVal * modVal);
                case EffectValueTypes.Subtract:
                    return statVal - modVal;
                case EffectValueTypes.SubtractMultiplier:
                    return statVal - (statVal * modVal);
                default:        // Add
                    return statVal + modVal;
            }
        }

        /// <summary>
        /// Get unchanging value of modifier
        /// </summary>
        /// <param name="statVal"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        private float GetSustainedValue(float statVal, DamageModifier stat)
        {
            float modVal = stat.Parent.GetExpressionValue(value);
            switch (valueType)
            {
                case EffectValueTypes.AddMultiplier:
                    return statVal + (statVal * modVal);
                case EffectValueTypes.Subtract:
                    return statVal - modVal;
                case EffectValueTypes.SubtractMultiplier:
                    return statVal - (statVal * modVal);
                default:        // Add
                    return statVal + modVal;
            }
        }

        #endregion

    }
}