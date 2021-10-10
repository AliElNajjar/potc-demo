using System;

public partial class UnitClass
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }
}