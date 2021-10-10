namespace NullSave.TOCK.Stats
{

    public enum EffectTypes
    {
        Instant = 0,
        Recurring = 1,
        Sustained = 2
    }

    public enum EffectValueTarget
    {
        Value = 0,
        MinimumValue = 1,
        MaximumValue = 2,
        RegenDelay = 3,
        RegenAmount = 4
    }

    public enum EffectValueTypes
    {
        Add = 0,
        AddMultiplier = 1,
        Subtract = 2,
        SubtractMultiplier = 3
    }

    public enum HitDirection
    {
        FrontLeft = 1,
        FrontCenter = 2,
        FrontRight = 4,
        Left = 8,
        Right = 16,
        BackLeft = 32,
        BackCenter = 64,
        BackRight = 128
    }

}