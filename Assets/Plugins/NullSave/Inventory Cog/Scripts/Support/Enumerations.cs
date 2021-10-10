namespace NullSave.TOCK.Inventory
{

    public enum AttachRequirement
    {
        NoneAllowed = 0,
        InCategory = 1,
        InItemList = 2
    }

    public enum AutoEquipMode
    {
        Never = 0,
        IfSlotFree = 1,
        Always = 2
    }

    public enum AutoEquipLocation
    {
        MirrorCurrent = 0,
        AlwaysEquip = 1,
        AlwaysStore = 2
    }

    public enum BooleanSource
    {
        Static = 0,
        StatExpression = 1
    }

    public enum ComponentListType
    {
        Breakdown = 0,
        Repair = 1
    }

    public enum CraftingTime
    {
        Instant = 0,
        GameTime = 1,
        RealTime = 2
    }

    public enum CraftingType
    {
        Create = 0,
        Upgrade = 1
    }

    [System.Flags] public enum CraftingUIColor
    {
        RecipeName = 1,
        Description = 2,
        Category = 4,
        Locked = 8,
        Time = 16,
        Icon = 32
    }

    [System.Flags]
    public enum ComponentUIColor
    {
        ComponentName = 1,
        Description = 2,
        Category = 4,
        Locked = 8,
        Time = 16,
        Icon = 32,
        CountNeeded = 64,
        CountAvailable = 128,
    }

    public enum ComponentType
    {
        Standard = 0,
        Advanced = 1
    }

    public enum CustomizationPointLocation
    {
        Hair = 0,
        Head = 1,
        ShoulderLeft = 2,
        ShoulderRight = 3,
        UpperArmLeft = 4,
        UpperArmRight = 5,
        LowerArmLeft = 6,
        LowerArmRight = 7,
        ArmLeft = 8,
        ArmRight = 9,
        Torso = 10,
        Legs = 11,
        UpperLegLeft = 12,
        UpperLegRight = 13,
        LowerLegLeft = 14,
        LowerLegRight = 15,
        LegLeft = 16,
        LegRight = 17,
        FootLeft = 18,
        FootRight = 19
    }

    public enum EquipState
    {
        NotEquipped = 0,
        Equipped = 1,
        Stored = 2
    }

    public enum ItemType
    {
        Ammo = 4,
        Armor = 2,
        Attachment = 10,
        Component = 8,
        Consumable = 0,
        Container = 9,
        Ingredient = 5,
        Journal = 7,
        QuestItem = 6,
        Shield = 3,
        Weapon = 1,
        Skill = 11,
    }

    public enum GenerationType
    {
        Default = 0,
        Constant = 1,
        RandomBetweenConstants = 2
    }

    public enum GridPageMode
    {
        AllAvailable = 0,
        OnlyUsed = 1
    }

    public enum InventorySortOrder
    {
        DisplayNameAsc = 1,
        DisplayNameDesc = 2,
        RarityAsc = 3,
        RarityDesc = 4,
        ValueAsc = 5,
        ValueDesc = 6,
        WeightAsc = 7,
        WeightDesc = 8,
        ConditionAsc = 9,
        ConditionDesc = 10,
        ItemTypeAsc = 11,
        ItemTypeDesc = 12,
        ItemCountAsc = 13,
        ItemCountDesc = 14
    }

    public enum ListCategoryFilter
    {
        All = 0,
        InList = 1,
        NotInList = 2,
    }

    public enum ListLoadMode
    {
        Manual = 0,
        OnEnable = 1
    }

    public enum ListSource
    {
        InventoryCog = 0,
        InventoryContainer = 1,
        InventoryMerchant = 2,
        ContainerItem = 3,
    }

    public enum NameModifierOrder
    {
        BeforeName = 0,
        AfterName = 1
    }

    public enum PageIndicatorMode
    {
        All = 0,
        AllUnlocked = 1,
        OnlyUsed = 2,
    }

    public enum PickupDetection
    {
        Trigger = 0,
        MainCamRaycast = 1
    }

    public enum PickupType
    {
        Manual = 0,
        Automatic = 1,
        ByButton = 2,
        ByKey = 3
    }

    public enum ResultType
    {
        Unmodified = 0,
        AverageOfComponents = 1,
        LowestOfComponents = 2,
        HighestOfComponents = 3
    }

    public enum TagFilterType
    {
        Matches = 0,
        Exists = 1,
        DoesNotExist = 2
    }

    public enum UpgradeResult
    {
        NoChange = 0,
        AddAmount = 1
    }

    public enum ValueSource
    {
        Static = 0,
        StatValue = 1
    }
}