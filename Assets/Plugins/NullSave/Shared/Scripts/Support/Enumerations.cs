namespace NullSave.TOCK
{

    public enum AudioNode
    {
        Music = 0,
        Ambient = 1,
    }

    public enum AnimatorParamType
    {
        Bool = 0,
        Float = 1,
        Int = 2,
        Trigger = 4
    }

    public enum AnimatorTriggerType
    {
        Set = 0,
        Reset = 1,
    }

    [System.Flags]
    public enum BasicDirection
    {
        Left = 1,
        Front = 2,
        Right = 4,
        Back = 8
    }

    public enum DamageModType
    {
        Resistance = 0,
        Weakness = 1
    }

    /// <summary>
    /// A more generic list of platforms
    /// </summary>
    [System.Flags]
    public enum GenericPlatform
    {
        Desktop = 0,
        Mobile = 1,
        Windows = 2,
        PS4 = 3,
        Switch = 4,
        XBOXOne = 5,
        OSX = 6,
        Linux = 7,
        Android = 8,
        IPhone = 9,
    }

    public enum MenuOpenType
    {
        ActiveGameObject = 0,
        SpawnOnFirstCanvas = 1,
        SpawnInTransform = 2,
        SpawnInTag = 3,
    }

    public enum NavigationType
    {
        Manual = 0,
        ByButton = 1,
        ByKey = 2
    }

}