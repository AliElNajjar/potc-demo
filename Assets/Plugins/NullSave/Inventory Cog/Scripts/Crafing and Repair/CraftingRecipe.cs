using NullSave.TOCK.Stats;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Crafting Recipe", order = 1)]
    public class CraftingRecipe : ScriptableObject
    {

        #region Variables

        // Shared Vars
        public Sprite icon;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public CraftingCategory craftingCategory;
        public bool unlocked = true;
        public bool displayInList = true;
        public float value;
        [Range(0, 1)] public float successChance = 1;

        [Range(0, 10)] public int rarity;

        public ValueSource valueSource = ValueSource.Static;
        public string valueStatName = "RecipeValue";
        private StatValue statValue;

        public ValueSource successSource = ValueSource.Static;
        public string successStatName = "RecipeSuccess";
        private StatValue statSuccess;

        public BooleanSource unlockedSource = BooleanSource.Static;
        public string unlockedExpression = "1 > 0";
        private bool unlockedCurVal;

        public BooleanSource displaySource = BooleanSource.Static;
        public string displayExpression = "1 > 0";
        private bool displayCurVal;

        private List<ExpressionSubscription> expressionSubscriptions;

        public CraftingType craftType = CraftingType.Create;
        public CraftingTime craftTime = CraftingTime.Instant;
        public float craftSeconds = 60;

        public ComponentType componentType = ComponentType.Standard;
        public List<ItemReference> components;
        public List<AdvancedComponent> advancedComponents;

        // Create Vars
        public List<CraftingResult> result;
        public List<CraftingResult> failResult;

        // Upgrade Vars
        public CraftingUpgradeReference baseItem;
        public CraftingUpgradeResult upgradeSuccess;
        public bool destoryOnFail = true;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public bool DisplayInList
        {
            get
            {
                if (displaySource == BooleanSource.Static) return displayInList;
                return displayCurVal;
            }
        }

        public InventoryCog InventoryCog { get; private set; }

        public System.DateTime? FirstCrafted { get; set; }

        public System.DateTime? LastCrafted { get; set; }

        public int FailCount { get; set; }

        public int SuccessCount { get; set; }

        public bool Unlocked
        {
            get
            {
                if (unlockedSource == BooleanSource.Static) return unlocked;
                return unlockedCurVal;
            }
        }

        #endregion

        #region Public Methods

        public float GetValue()
        {
            if (valueSource == ValueSource.Static) return value;
            if (statValue == null)
            {
                if (InventoryCog.StatsCog == null)
                {
                    Debug.LogWarning(name + ".GetValue, no StatsCog available");
                    return 0;
                }

                statValue = InventoryCog.StatsCog.FindStat(valueStatName);
                if (statValue == null)
                {
                    Debug.LogWarning(name + ".GetValue, cannot find stat " + valueStatName);
                    return 0;
                }
            }

            return statValue.CurrentValue;
        }

        public float GetSuccessChance()
        {
            if (successSource == ValueSource.Static) return successChance;

            if (statSuccess == null)
            {
                if (InventoryCog.StatsCog == null)
                {
                    Debug.LogWarning(name + ".GetSuccessChance, no StatsCog available");
                    return 0;
                }

                statSuccess = InventoryCog.StatsCog.FindStat(successStatName);
                if (statSuccess == null)
                {
                    Debug.LogWarning(name + ".GetSuccessChance, cannot find stat " + successStatName);
                    return 0;
                }
            }

            return statSuccess.CurrentValue;
        }

        public void Initialize(InventoryCog inventoryCog)
        {
            InventoryCog = inventoryCog;
            FirstCrafted = LastCrafted = null;
            SuccessCount = FailCount = 0;
            RemoveSubscriptions();
            AddSubscriptions();
        }

        public void StateLoad(Stream stream)
        {
            RemoveSubscriptions();
            FailCount = stream.ReadInt();
            SuccessCount = stream.ReadInt();
            if (SuccessCount > 0)
            {
                FirstCrafted = new System.DateTime(stream.ReadLong());
                LastCrafted = new System.DateTime(stream.ReadLong());
            }
            AddSubscriptions();
        }

        public void StateSave(Stream stream)
        {
            stream.WriteInt(FailCount);
            stream.WriteInt(SuccessCount);
            if (SuccessCount > 0)
            {
                stream.WriteLong(((System.DateTime)FirstCrafted).Ticks);
                stream.WriteLong(((System.DateTime)LastCrafted).Ticks);
            }
        }

        #endregion

        #region Private Methods

        private void AddSubscriptions()
        {
            if (expressionSubscriptions == null) expressionSubscriptions = new List<ExpressionSubscription>();

            if (displaySource == BooleanSource.StatExpression)
            {
                System.Action<bool> displaySub = (bool result) => { displayCurVal = result; };
                ExpressionSubscription sub = new ExpressionSubscription(displayExpression, displaySub);
                sub.Subscribe(InventoryCog.StatsCog);
                expressionSubscriptions.Add(sub);
            }

            if (unlockedSource == BooleanSource.StatExpression)
            {
                System.Action<bool> unlockedSub = (bool result) => { unlockedCurVal = result; };
                ExpressionSubscription sub = new ExpressionSubscription(unlockedExpression, unlockedSub);
                sub.Subscribe(InventoryCog.StatsCog);
                expressionSubscriptions.Add(sub);
            }
        }

        private void RemoveSubscriptions()
        {
            if (expressionSubscriptions == null) return;

            foreach (ExpressionSubscription sub in expressionSubscriptions)
            {
                sub.Unsubscribe();
            }

            expressionSubscriptions.Clear();
        }

        #endregion

    }
}