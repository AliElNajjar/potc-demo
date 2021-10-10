public interface IWearable
{
    float Level { get; set; }
    float Weight { get; set; }
}

public enum ElementalUnit
{
    None = 0,
    Health = 1,
    Mind = 2,
    Time = 3,
    Sound = 4,
    Soul = 5
}

public enum PhysicUnit
{
    None = 0,
    Slash = 1,
    Crush = 2,
    Pierce = 3,
}