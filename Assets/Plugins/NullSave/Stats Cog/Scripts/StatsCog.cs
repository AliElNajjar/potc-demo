using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    // Enable heirarchy icon w/ bubble-up
    [HierarchyIcon("statscog_icon", "#ffffff")]
    [DefaultExecutionOrder(-101)]
    public class StatsCog : MonoBehaviour
    {

        #region Variables

        public List<StatValue> stats;
        public StatEffectList effectList;
        public List<StatEffect> startingEffects;
        public List<EffectResistance> effectResistances;

        public string healthStat = "HP";
        public string damageValue = "[Damage]";
        public HitDirection directionImmunity;
        public List<DamageModifier> damageModifiers;
        public float immunityAfterHit = 0.5f;
        public float hitDirTolerance = 0.15f;

        public DamageTaken onDamageTaken;
        public UnityEvent onImmuneToDamage, onDeath;
        public EffectAdded onEffectAdded;
        public EffectEnded onEffectEnded;
        public EffectRemoved onEffectRemoved;
        public EffectResisted onEffectResisted;
        public Impacted onHitDirection, onHitDamageDirection;
        public LayerMask ignoreLayer = 0;

        private List<StatEffect> deadEffects;
        private float immuneRemaining;
        private EffectArea lastArea;
        private StatsCog lastDmgSource;

        public int z_display_flags = 4095;
        public string effectFolder, statFolder;

        #endregion

        #region Properties

        /// <summary>
        /// Returns a list of active status effects
        /// </summary>
        public List<StatEffect> Effects { get; private set; }

        /// <summary>
        /// Returns a list of active stats
        /// </summary>
        public List<StatValue> Stats { get; private set; }

        #endregion

        #region Unity Methods

        public void Awake()
        {
            // Create new lists
            Stats = new List<StatValue>();
            Effects = new List<StatEffect>();
            deadEffects = new List<StatEffect>();

            // Update dealers
            UpdateDamageDealers();

            // Instance stats
            foreach (StatValue stat in stats)
            {
                StatValue instance = InstanceStatValue(stat);
                Stats.Add(instance);
            }

            // Initialize Stats
            foreach (StatValue stat in Stats)
            {
                stat.Initialize(this);
            }

            // Initailize Damage Modifiers
            foreach (DamageModifier modifer in damageModifiers)
            {
                modifer.Initialize(this);
            }

            // Initialize Starting Effects
            foreach (StatEffect effect in startingEffects)
            {
                AddEffect(effect);
            }
        }

        public void Update()
        {
            // Effect Updates
            foreach (StatEffect effect in Effects)
            {
                effect.Update();
                if (effect.IsDead)
                {
                    deadEffects.Add(effect);
                }
            }

            // Remove dead effects
            foreach (StatEffect effect in deadEffects)
            {
                EndEffect(effect);
            }
            deadEffects.Clear();

            // Stat Updates
            foreach (StatValue stat in Stats)
            {
                if (stat.name == healthStat)
                {
                    float preUpdate = stat.CurrentValue;
                    stat.Update();
                    if (stat.CurrentValue < preUpdate)
                    {
                        onDamageTaken?.Invoke(preUpdate - stat.CurrentValue, null, null);
                    }
                }
                else
                {
                    stat.Update();
                }
            }

            // Update Damage Modifiers
            foreach (DamageModifier modifer in damageModifiers)
            {
                modifer.Update();
            }

            // Update hit immunity
            if (immuneRemaining > 0)
            {
                immuneRemaining = Mathf.Clamp(immuneRemaining - Time.deltaTime, 0, immuneRemaining);
            }
        }

        public void OnEnable()
        {
            DamageReceiver[] drs = GetComponentsInChildren<DamageReceiver>();
            foreach (DamageReceiver dr in drs)
            {
                dr.StatsParent = this;
                dr.onTakeDamage.AddListener(TakeDamage);
            }
        }

        private void OnDisable()
        {
            DamageReceiver[] drs = GetComponentsInChildren<DamageReceiver>();
            foreach (DamageReceiver dr in drs)
            {
                dr.StatsParent = null;
                dr.onTakeDamage.RemoveListener(TakeDamage);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            EffectArea[] ea = other.GetComponentsInChildren<EffectArea>();
            foreach (EffectArea area in ea)
            {
                if (area != null && area.enabled && area != lastArea)
                {
                    if (area.areaEffect != null)
                    {
                        AddEffect(area.areaEffect);
                    }
                    if (area.cancelEffects != null)
                    {
                        foreach (StatEffect effect in area.cancelEffects)
                        {
                            RemoveEffect(effect);
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            EffectArea ea = other.GetComponentInChildren<EffectArea>();
            if (ea != null && ea.enabled && ea.areaEffect != null)
            {
                lastArea = ea;
                if (ea.removeOnExit)
                {
                    RemoveEffect(ea.areaEffect);
                }
                StartCoroutine("EndAreaEffect");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            EffectArea[] ea = other.GetComponentsInChildren<EffectArea>();
            foreach (EffectArea area in ea)
            {
                if (area != null && area.enabled && area != lastArea && area.areaEffect != null && area.reAddOnStay)
                {
                    AddEffect(area.areaEffect);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a new active effect
        /// </summary>
        /// <param name="effectName"></param>
        public void AddEffect(string effectName)
        {
            foreach (StatEffect effect in effectList.availableEffects)
            {
                if (effect.name == effectName)
                {
                    AddEffect(effect);
                    return;
                }
            }

            Debug.LogWarning(name + ": Could not find effect '" + effectName + "'");
        }

        /// <summary>
        /// Add a new active effect
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffect(StatEffect effect)
        {
            StatValue stat;
            bool addToActive = false;

            // Check for stack issues
            if (!effect.canStack)
            {
                foreach (StatEffect fx in Effects)
                {
                    if (fx.name == effect.name)
                    {
                        if (fx.resetLifeOnAdd)
                        {
                            fx.RemainingTime = fx.lifeInSeconds;
                        }
                        return;
                    }
                }
            }

            // Check if effect is prevented
            foreach (StatEffect fx in Effects)
            {
                if (fx.IsEffectPrevented(effect.name))
                {
                    return;
                }
            }

            // Check for resist
            foreach (EffectResistance effectResistance in effectResistances)
            {
                if (effectResistance.effect.name == effect.name)
                {
                    float resistRoll = Random.Range(0f, 1f);

                    if (resistRoll <= effectResistance.resistChance)
                    {
                        onEffectResisted?.Invoke(effect);
                        return;
                    }

                    break;
                }
            }

            // Remove cancelled & prevented effects
            List<StatEffect> removeByNew = new List<StatEffect>();
            foreach (StatEffect fx in Effects)
            {
                if (!removeByNew.Contains(fx) && effect.IsEffectCancelled(fx.name) || effect.IsEffectPrevented(fx.name))
                {
                    removeByNew.Add(fx);
                }
            }

            foreach (StatEffect fx in removeByNew)
            {
                RemoveEffect(fx);
            }

            // Find modifier targets
            foreach (StatModifier mod in effect.modifiers)
            {
                stat = FindStat(mod.affectedStat);
                if (stat == null)
                {
                    // Apply to modifiers
                    List<DamageModifier> modifiers = FindDamageModifiers(mod.affectedStat);
                    foreach (DamageModifier dm in modifiers)
                    {
                        dm.AddModifier(mod);
                        if (mod.effectType != EffectTypes.Instant)
                        {
                            addToActive = true;
                        }
                    }
                }
                else
                {
                    stat.AddModifier(mod);
                    if (mod.effectType != EffectTypes.Instant)
                    {
                        addToActive = true;
                    }
                }
            }

            // Check for duration only items
            if (!addToActive && (effect.hasLifeSpan || effect.preventEffects.Count > 0))
            {
                if (!effect.canStack)
                {
                    bool hasInstance = false;
                    foreach (StatEffect fx in Effects)
                    {
                        if (fx.name == effect.name)
                        {
                            hasInstance = true;
                            break;
                        }
                    }

                    if (!hasInstance) addToActive = true;
                }
                else
                {
                    addToActive = true;
                }
            }

            if (addToActive)
            {
                StatEffect instance = InstanceStatEffect(effect);
                instance.Initialize();
                Effects.Add(instance);
                onEffectAdded?.Invoke(instance);
            }
        }

#if INVENTORY_COG

        public void AddInventoryEffects(Inventory.InventoryItem item)
        {
            // Base Effects
            foreach (StatEffect effect in item.statEffects)
            {
                AddEffect(effect);
            }

            // Attachment Effects
            if (item.itemType != Inventory.ItemType.Attachment && item.attachRequirement != Inventory.AttachRequirement.NoneAllowed)
            {
                foreach (Inventory.AttachmentSlot slot in item.Slots)
                {
                    if (slot.AttachedItem != null)
                    {
                        AddInventoryEffects(slot.AttachedItem);
                    }
                }
            }
        }

        public void AddInventoryEffects(Inventory.InventoryItem item, int count)
        {
            int i;
            foreach (StatEffect effect in item.statEffects)
            {
                for (i = 0; i < count; i++)
                {
                    AddEffect(effect);
                }
            }

            // Attachment Effects
            if (item.itemType != Inventory.ItemType.Attachment && item.attachRequirement != Inventory.AttachRequirement.NoneAllowed)
            {
                foreach (Inventory.AttachmentSlot slot in item.Slots)
                {
                    if (slot.AttachedItem != null)
                    {
                        AddInventoryEffects(slot.AttachedItem, count);
                    }
                }
            }
        }

#else
        public void AddInventoryEffects(object item) { throw new System.NotImplementedException("Requires Inventory Cog"); }

        public void AddInventoryEffects(object item, int count) { throw new System.NotImplementedException("Requires Inventory Cog"); }

#endif

        /// <summary>
        /// Clear all active effects
        /// </summary>
        public void ClearEffects()
        {
            while (Effects.Count > 0)
            {
                RemoveEffect(Effects[0]);
            }
        }

        /// <summary>
        /// Evaluate if an expression is true
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool EvaluateCondition(string expression)
        {
            List<string> parts = new List<string>();

            int andIndex, orIndex, idx;
            while (true)
            {
                andIndex = expression.IndexOf("&&");
                orIndex = expression.IndexOf("||");
                if (andIndex == -1 && orIndex == -1) break;

                if (andIndex == -1) andIndex = int.MaxValue;
                if (orIndex == -1) orIndex = int.MaxValue;
                idx = Mathf.Min(andIndex, orIndex);

                parts.Add(expression.Substring(0, idx));
                parts.Add(expression.Substring(idx, 2));
                expression = expression.Substring(idx + 2);
            }
            parts.Add(expression);

            bool res = false;
            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] == "&&")
                {
                    res = res && EvalCondition(parts[i + 1]);
                    i += 1;
                }
                else if (parts[i] == "||")
                {
                    res = res || EvalCondition(parts[i + 1]);
                    i += 1;
                }
                else
                {
                    res = EvalCondition(parts[i]);
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the value of an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public float GetExpressionValue(string expression)
        {
            if (expression.IsNumeric())
            {
                return float.Parse(expression, System.Globalization.CultureInfo.InvariantCulture);
            }

            return ParseStatValue(expression);
        }

        /// <summary>
        /// Get a list of stats needed for equation
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
        public List<string> GetSubscriptionRequirements(string equation)
        {
            List<string> res = new List<string>();
            if (!equation.IsNumeric())
            {
                equation = equation.Replace("(", "( ");
                equation = equation.Replace(")", " )");

                string[] parts = equation.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();

                    if (!parts[i].IsNumeric() && !LogicExtensions.BASIC_EXPRESSIONS.Contains(parts[i]))
                    {
                        res.Add(parts[i]);
                    }
                }
            }
            return res;
        }

        public List<DamageModifier> FindDamageModifiers(string modifierName)
        {
            List<DamageModifier> result = new List<DamageModifier>();
            foreach (DamageModifier modifier in damageModifiers)
            {
                if (modifier.name == modifierName)
                {
                    result.Add(modifier);
                }
            }

            return result;
        }

        public List<DamageModifier> FindDamageModifiers(DamageModifier modifier)
        {
            return FindDamageModifiers(modifier.name);
        }

        /// <summary>
        /// Returns active instance of a stat
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        public StatValue FindStat(string statName)
        {
            if (Stats == null) return null;

            foreach (StatValue stat in Stats)
            {
                if (stat.name == statName)
                {
                    return stat;
                }
            }

            return null;
        }

        /// <summary>
        /// Load data from a file
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            using (FileStream fs = new FileStream(Application.persistentDataPath + "\\" + filename, FileMode.Open, FileAccess.Read))
            {
                Load(fs);
            }
        }

        /// <summary>
        /// Load data from a filestream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            if (stream.Position == stream.Length) return;

            float version = stream.ReadFloat();

            switch (version)
            {
                case 1.4f:
                    Version1_4_Load(stream);
                    break;
                default:
                    Debug.LogWarning("Invalid StatsCog save format version '" + version + "'");
                    break;
            }
        }

        /// <summary>
        /// End an active effect
        /// </summary>
        /// <param name="effectName"></param>
        public void EndEffect(string effectName)
        {
            foreach (StatEffect effect in Effects)
            {
                if (effect.name == effectName)
                {
                    EndEffect(effect);
                    return;
                }
            }
        }

        /// <summary>
        /// End an active effect
        /// </summary>
        /// <param name="effect"></param>
        public void EndEffect(StatEffect effect)
        {
            StatValue stat;
            List<DamageModifier> modifiers;

            // Find modifier targets
            foreach (StatModifier mod in effect.modifiers)
            {
                stat = FindStat(mod.affectedStat);
                if (stat != null)
                {
                    stat.RemoveModifier(mod);
                }
                modifiers = FindDamageModifiers(mod.affectedStat);
                foreach (DamageModifier dm in modifiers)
                {
                    dm.RemoveModifier(mod);
                }
            }

            if (effect.effectParticles != null)
            {
                Destroy(effect.effectParticles);
            }

            Effects.Remove(effect);
            onEffectEnded?.Invoke(effect);
            effect.onEnd?.Invoke();
        }

        /// <summary>
        /// Get a list of active effects by category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<StatEffect> GetEffectsByCategory(string categoryName)
        {
            List<StatEffect> result = new List<StatEffect>();
            foreach (StatEffect effect in Effects)
            {
                if (effect.category == categoryName)
                {
                    result.Add(effect);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate the change in value when replacing one modifier with another
        /// </summary>
        /// <param name="original">Original Stat Modifier</param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public float GetModifierChange(StatModifier original, StatModifier replacement)
        {
            // Get stat
            StatValue stat = FindStat(original.affectedStat);
            return stat.GetModifierChange(original, replacement);
        }

        /// <summary>
        /// Get a list of stat values by category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<StatValue> GetValuesByCategory(string categoryName)
        {
            List<StatValue> result = new List<StatValue>();
            foreach (StatValue value in Stats)
            {
                if (value.category == categoryName)
                {
                    result.Add(value);
                }
            }
            return result;
        }

        /// <summary>
        /// Remove all active effects flagged as benificial (positive/helpful)
        /// </summary>
        public void RemoveBenificialEffects()
        {
            List<StatEffect> toRemove = new List<StatEffect>();
            foreach (StatEffect effect in Effects)
            {
                if (effect.isBenificial)
                {
                    toRemove.Add(effect);
                }
            }

            foreach (StatEffect effect in toRemove)
            {
                RemoveEffect(effect);
            }
        }

        /// <summary>
        /// Remove all active effects in a category
        /// </summary>
        /// <param name="category"></param>
        public void RemoveEffectsByCategory(string category)
        {
            List<StatEffect> toRemove = new List<StatEffect>();
            foreach (StatEffect effect in Effects)
            {
                if (effect.category == category)
                {
                    toRemove.Add(effect);
                }
            }

            foreach (StatEffect effect in toRemove)
            {
                RemoveEffect(effect);
            }
        }

        /// <summary>
        /// Remove an active effect
        /// </summary>
        /// <param name="effectName"></param>
        public void RemoveEffect(string effectName)
        {
            foreach (StatEffect effect in Effects)
            {
                if (effect.name == effectName)
                {
                    RemoveEffect(effect);
                    return;
                }
            }
        }

        /// <summary>
        /// Remove an active effect
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveEffect(StatEffect effect)
        {
            if (!Effects.Contains(effect))
            {
                foreach (StatEffect fx in Effects)
                {
                    if (fx.name == effect.name)
                    {
                        RemoveEffect(fx);
                        return;
                    }
                }

                return;
            }

            if (!effect.isRemoveable) return;

            StatValue stat;
            List<DamageModifier> modifiers;

            // Find modifier targets
            foreach (StatModifier mod in effect.modifiers)
            {
                stat = FindStat(mod.affectedStat);
                if (stat != null)
                {
                    stat.RemoveModifier(mod);
                }
                modifiers = FindDamageModifiers(mod.affectedStat);
                foreach (DamageModifier dm in modifiers)
                {
                    dm.RemoveModifier(mod);
                }
            }

            if (effect.effectParticles != null)
            {
                Destroy(effect.effectParticles);
            }

            Effects.Remove(effect);
            onEffectRemoved?.Invoke(effect);
            effect.onEnd?.Invoke();
        }

        /// <summary>
        /// Remove all active effects
        /// </summary>
        /// <param name="effectName"></param>
        public void RemoveEffectAll(string effectName)
        {
            bool found = false;

            while (true)
            {
                found = false;

                foreach (StatEffect effect in Effects)
                {
                    if (effect.name == effectName)
                    {
                        RemoveEffect(effect);
                        found = true;
                        break;
                    }
                }

                if (!found) return;
            }
        }

        /// <summary>
        /// Remove all active effects flagged as detrimental (negative/harmful)
        /// </summary>
        public void RemoveDetrimentalEffects()
        {
            List<StatEffect> toRemove = new List<StatEffect>();
            foreach (StatEffect effect in Effects)
            {
                if (effect.isDetrimental)
                {
                    toRemove.Add(effect);
                }
            }

            foreach (StatEffect effect in toRemove)
            {
                RemoveEffect(effect);
            }
        }

#if INVENTORY_COG

        public void RemoveInventoryEffects(Inventory.InventoryItem item)
        {
            // Base stats
            foreach (StatEffect effect in item.statEffects)
            {
                RemoveEffect(effect);
            }

            // Attachment Effects
            if (item.itemType != Inventory.ItemType.Attachment && item.attachRequirement != Inventory.AttachRequirement.NoneAllowed)
            {
                foreach (Inventory.AttachmentSlot slot in item.Slots)
                {
                    if (slot.AttachedItem != null)
                    {
                        RemoveInventoryEffects(slot.AttachedItem);
                    }
                }
            }
        }
#else
        public void RemoveInventoryEffects(object item) { throw new System.NotImplementedException("Requires Inventory Cog"); }
#endif

        /// <summary>
        /// Remove all affects without benificial or detrimental flags
        /// </summary>
        public void RemoveNeutralEffects()
        {
            List<StatEffect> toRemove = new List<StatEffect>();
            foreach (StatEffect effect in Effects)
            {
                if (!effect.isBenificial && !effect.isDetrimental)
                {
                    toRemove.Add(effect);
                }
            }

            foreach (StatEffect effect in toRemove)
            {
                RemoveEffect(effect);
            }
        }

        /// <summary>
        /// Save state to file
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using (FileStream fs = new FileStream(Application.persistentDataPath + "\\" + filename, FileMode.Create, FileAccess.Write))
            {
                Save(fs);
            }
        }

        /// <summary>
        /// Save state to filestream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            // Write file format version
            stream.WriteFloat(1.4f);

            foreach (StatValue stat in Stats)
            {
                stat.Save(stream);
            }

            stream.WriteInt(damageModifiers.Count);
            foreach (DamageModifier damageModifier in damageModifiers)
            {
                damageModifier.Save(stream);
            }

            stream.WriteInt(Effects.Count);
            foreach (StatEffect effect in Effects)
            {
                stream.WriteStringPacket(effect.name);
                stream.WriteFloat(effect.RemainingTime);
            }
        }

        /// <summary>
        /// Send a command to StatsCog
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            command = command.Replace("  ", " ");
            int i = command.IndexOf(' ');
            int e = command.IndexOf('=');
            float res;

            switch (command.Substring(0, i).Trim().ToLower())
            {
                case "add":         // Add effect
                    AddEffect(command.Substring(i).Trim());
                    break;
                case "remove":      // Remove effect
                    RemoveEffect(command.Substring(i).Trim());
                    break;
                case "removeall":   // Remove effect all
                    RemoveEffectAll(command.Substring(i).Trim());
                    break;
                case "clear":       // Clear effects
                    ClearEffects();
                    break;
                case "max":         // Set max value
                    res = ParseStatValue(command.Substring(e + 1));
                    FindStat(command.Substring(i, e - i - 1).Trim()).SetMaximum(res);
                    break;
                case "min":         // Set min value
                    res = ParseStatValue(command.Substring(e + 1));
                    FindStat(command.Substring(i, e - i - 1).Trim()).SetMinimum(res);
                    break;
                case "restore-min": // Retore value to a minimum of passed value
                    e = command.IndexOf(' ', i + 1);
                    res = ParseStatValue(command.Substring(e + 1).Trim());
                    StatValue val = FindStat(command.Substring(i, e - i).Trim());
                    if (val != null)
                    {
                        if (val.CurrentValue < res)
                        {
                            val.SetValue(res);
                        }
                    }
                    break;
                case "setmax":      // Set value to maximum
                    StatValue stat = FindStat(command.Substring(i).Trim());
                    if (stat != null)
                    {
                        stat.SetValue(stat.CurrentBaseMaximum);
                    }
                    break;
                case "value":       // Set value
                    res = ParseStatValue(command.Substring(e + 1));
                    FindStat(command.Substring(i, e - i - 1).Trim()).SetValue(res);
                    break;
            }
        }

        public void SendCommandToLastDamageDealer(string command)
        {
            if (lastDmgSource != null)
            {
                lastDmgSource.SendCommand(command);
            }
        }

        /// <summary>
        /// Take damage
        /// </summary>
        /// <param name="damageDealer"></param>
        /// <returns></returns>
        private void TakeDamage(DamageDealer damageDealer, GameObject damageSourceObject)
        {
            // Check for ignore layer
            if (ignoreLayer == (ignoreLayer | (1 << damageDealer.gameObject.layer))) return;

            // check if damage dealer is self
            if (GetComponentsInChildren<DamageDealer>().ToList().Contains(damageDealer))
            {
                return;
            }

            // Get hit direction
            HitDirection hitDirection = GetHitDirection(damageDealer.gameObject.transform.position);

            // Check for immunity
            if (directionImmunity != 0 && (directionImmunity | hitDirection) == hitDirection)
            {
                onImmuneToDamage?.Invoke();
                return;
            }

            if (immuneRemaining > 0)
            {
                return;
            }

            // Add Effects
#if STATS_COG
            if (damageDealer.effects != null)
            {
                foreach (StatEffect effect in damageDealer.effects)
                {
                    if (effectList.availableEffects.Contains(effect))
                    {
                        AddEffect(effect);
                    }
                }
            }
#endif

            float adjustedDamage = 0;
            float totalAdjustedDamage = 0;

            // Apply weakness
            foreach (Damage damage in damageDealer.damage)
            {

#if STATS_COG
                if (damageDealer.StatsSource != null)
                {
                    adjustedDamage = damageDealer.StatsSource.GetExpressionValue(damage.baseAmount);
                }
                else
                {
                    adjustedDamage = float.Parse(damage.baseAmount, System.Globalization.CultureInfo.InvariantCulture);
                }
#endif
                if (damage.damageType != null)
                {
                    List<DamageModifier> modifiers = FindDamageModifiers(damage.damageType.name);
                    foreach (DamageModifier dm in modifiers)
                    {
                        if (dm.modifierType == DamageModType.Resistance)
                        {
                            adjustedDamage -= adjustedDamage * dm.CurrentValue;
                            if (dm.CurrentValue == 1)
                            {
                                onImmuneToDamage?.Invoke();
                            }
                        }
                        else
                        {
                            adjustedDamage *= dm.CurrentValue;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Damage is missing a DamageType");
                }

                totalAdjustedDamage += adjustedDamage;
            }

            // Apply damage
            StatValue hp = FindStat(healthStat);
            if (hp != null && hp.CurrentValue > 0)
            {
                adjustedDamage = Mathf.Clamp(GetExpressionValue(ReplaceInsensitive(damageValue, "[damage]", totalAdjustedDamage.ToString())), 0, float.MaxValue);
                hp.SetValue(hp.CurrentValue - adjustedDamage);
                lastDmgSource = damageDealer.StatsSource;
                onDamageTaken?.Invoke(adjustedDamage, damageDealer, damageSourceObject);
                onHitDamageDirection?.Invoke(hitDirection);
                immuneRemaining = immunityAfterHit;
                onHitDirection?.Invoke(hitDirection);
                if (hp.CurrentValue <= 0)
                {
                    onDeath?.Invoke();
                }
            }
            else
            {
                Debug.LogWarning("Could not find '" + healthStat + "' to apply damage");
            }

        }

        /// <summary>
        /// Update all damage dealers in children
        /// </summary>
        public void UpdateDamageDealers()
        {
            foreach (DamageShield shield in gameObject.GetComponentsInChildren<DamageShield>())
            {
                shield.StatsSource = this;
            }

            foreach (DamageDealer dealer in gameObject.GetComponentsInChildren<DamageDealer>())
            {
                dealer.StatsSource = this;
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator EndAreaEffect()
        {
            yield return new WaitForEndOfFrame();
            lastArea = null;
        }

        private bool EvalCondition(string equation)
        {
            string check = "=";

            int ei = equation.IndexOf('=');
            if (ei > 0)
            {

                if (equation.Substring(ei - 1, 1) == "<") check = "<=";
                if (equation.Substring(ei - 1, 1) == ">") check = ">=";

                float v1 = ParseStatValue(equation.Substring(0, ei - check.Length));
                float v2 = ParseStatValue(equation.Substring(ei + 1));

                switch (check)
                {
                    case "<=":
                        return v1 <= v2;
                    case ">=":
                        return v1 >= v2;
                    default:
                        return v1 == v2;
                }
            }

            check = "<";
            ei = equation.IndexOf('<');
            if (ei > 0)
            {
                float v1 = ParseStatValue(equation.Substring(0, ei - check.Length));
                float v2 = ParseStatValue(equation.Substring(ei + 1));
                return v1 < v2;
            }

            check = ">";
            ei = equation.IndexOf('>');
            if (ei > 0)
            {
                float v1 = ParseStatValue(equation.Substring(0, ei - check.Length));
                float v2 = ParseStatValue(equation.Substring(ei + 1));
                return v1 > v2;
            }

            return false;
        }

        private HitDirection GetHitDirection(Vector3 OtherObject)
        {
            if (Vector3.Dot(transform.forward, OtherObject - transform.position) < -hitDirTolerance)
            {
                // Back
                if (Vector3.Dot(transform.right, OtherObject - transform.position) < -hitDirTolerance) return HitDirection.BackLeft;
                if (Vector3.Dot(transform.right, OtherObject - transform.position) > hitDirTolerance) return HitDirection.BackRight;
                return HitDirection.BackCenter;
            }
            else if (Vector3.Dot(transform.forward, OtherObject - transform.position) > hitDirTolerance)
            {
                // Front
                if (Vector3.Dot(transform.right, OtherObject - transform.position) < -hitDirTolerance) return HitDirection.FrontLeft;
                if (Vector3.Dot(transform.right, OtherObject - transform.position) > hitDirTolerance) return HitDirection.FrontRight;
                return HitDirection.FrontCenter;
            }
            else
            {
                // Side
                if (Vector3.Dot(transform.right, OtherObject - transform.position) < -hitDirTolerance) return HitDirection.Left;
                return HitDirection.Right;
            }
        }

        private StatValue InstanceStatValue(StatValue stat)
        {
            StatValue instance = (StatValue)ScriptableObject.CreateInstance("StatValue");

            instance.value = stat.value;
            instance.minValue = stat.minValue;
            instance.maxValue = stat.maxValue;
            instance.startWithMaxValue = stat.startWithMaxValue;
            instance.category = stat.category;
            instance.displayInList = stat.displayInList;

            instance.enableRegen = stat.enableRegen;
            instance.regenDelay = stat.regenDelay;
            instance.regenPerSecond = stat.regenPerSecond;

            instance.enableIncrement = stat.enableIncrement;
            instance.incrementWhen = stat.incrementWhen;
            instance.incrementAmount = stat.incrementAmount;
            instance.incrementCommand = stat.incrementCommand;

            instance.onBaseMaxValueChanged = new ValueChanged();
            instance.onBaseMinValueChanged = new ValueChanged();
            instance.onBaseValueChanged = new ValueChanged();
            instance.onInit = new UnityEvent();
            instance.onMaxValueChanged = new ValueChanged();
            instance.onMinValueChanged = new ValueChanged();
            instance.onValueChanged = new ValueChanged();

            instance.icon = stat.icon;
            instance.iconColor = stat.iconColor;
            instance.displayName = stat.displayName;
            instance.textColor = stat.textColor;

            instance.name = stat.name;

            return instance;
        }

        private StatEffect InstanceStatEffect(StatEffect effect)
        {
            StatEffect instance = (StatEffect)ScriptableObject.CreateInstance("StatEffect");

            instance.displayName = effect.displayName;
            instance.description = effect.description;
            instance.sprite = effect.sprite;
            instance.displayInList = effect.displayInList;
            instance.category = effect.category;

            instance.canStack = effect.canStack;
            instance.resetLifeOnAdd = effect.resetLifeOnAdd;

            instance.startedText = effect.startedText;
            instance.endedText = effect.endedText;
            instance.removedText = effect.removedText;
            instance.isBenificial = effect.isBenificial;
            instance.isDetrimental = effect.isDetrimental;
            instance.isRemoveable = effect.isRemoveable;

            instance.hasLifeSpan = effect.hasLifeSpan;
            instance.lifeInSeconds = effect.lifeInSeconds;
            instance.modifiers = effect.modifiers;
            instance.cancelEffects = effect.cancelEffects;
            instance.preventEffects = effect.preventEffects;

            instance.onEnd = new UnityEvent();
            instance.onStart = new UnityEvent();

            instance.name = effect.name;

            if (effect.effectParticles != null)
            {
                GameObject go = Instantiate(effect.effectParticles, transform);
                instance.effectParticles = go;
            }

            return instance;
        }

        private float ParseStatValue(string equation)
        {
            equation = equation.Replace("(", "( ");
            equation = equation.Replace(")", " )");

            StatValue stat;
            string[] parts = equation.Split(' ');
            string eval = string.Empty;

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();

                if (parts[i].IsNumeric())
                {
                    eval += float.Parse(parts[i], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                }
                else if (LogicExtensions.FULL_EXPRESSIONS.Contains(parts[i]))
                {
                    eval += parts[i];
                }
                else
                {
                    stat = FindStat(parts[i]);
                    if (stat == null)
                    {
                        Debug.LogError("Could not locate '" + parts[i] + "' in: " + equation);
                    }
                    else
                    {
                        // Make sure we've initialized the stat
                        if (!stat.Initialized)
                        {
                            stat.Initialize(this);
                        }

                        eval += stat.CurrentValue.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            return eval.EvaluateSimpleMath();
        }

        private string ReplaceInsensitive(string source, string find, string replaceWith)
        {
            find = find.ToLower();
            string lowerSource = source.ToLower();
            int i;

            while (true)
            {
                i = lowerSource.IndexOf(find);
                if (i < 0)
                {
                    return source;
                }

                if (i > 0)
                {
                    source = source.Substring(0, i) + replaceWith + source.Substring(i + find.Length);
                }
                else
                {
                    source = replaceWith + source.Substring(i + find.Length);
                }
                lowerSource = source.ToLower();
            }
        }

        private void Version1_4_Load(Stream stream)
        {
            int i, count;
            string effectName;

            // Remove all current effects
            foreach (StatEffect effect in Effects)
            {
                onEffectRemoved?.Invoke(effect);
            }

            foreach (StatValue stat in Stats)
            {
                stat.Load(stream, 1.4f);
            }

            damageModifiers.Clear();
            count = stream.ReadInt();
            for( i=0; i < count; i++)
            {
                DamageModifier instance = ScriptableObject.CreateInstance<DamageModifier>();
                instance.Load(stream, 1.4f);
                damageModifiers.Add(instance);
            }

            Effects.Clear();
            count = stream.ReadInt();
            for (i = 0; i < count; i++)
            {
                effectName = stream.ReadStringPacket();
                foreach (StatEffect effect in effectList.availableEffects)
                {
                    if (effect.name == effectName)
                    {
                        StatEffect instance = InstanceStatEffect(effect);
                        instance.RemainingTime = stream.ReadFloat();
                        Effects.Add(instance);
                        onEffectAdded?.Invoke(instance);

                        break;
                    }
                }
            }
        }

        #endregion

    }
}