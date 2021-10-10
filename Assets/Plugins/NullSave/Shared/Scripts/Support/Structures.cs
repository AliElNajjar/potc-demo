using UnityEngine;

namespace NullSave.TOCK
{

    [System.Serializable]
    public struct BinaryValue
    {
        public string Name;
        public byte[] Value;
    }

    [System.Serializable]
    public struct BoolValue
    {
        public string Name;
        public bool Value;
    }

    [System.Serializable]
    public struct FloatValue
    {
        public string Name;
        public float Value;
    }

    [System.Serializable]
    public struct IntValue
    {
        public string Name;
        public int Value;
    }

    [System.Serializable]
    public struct LongValue
    {
        public string Name;
        public long Value;
    }


    [System.Serializable]
    public struct ObjectValue
    {
        public string Name;
        public object Value;
    }

    [System.Serializable]
    public struct QuaternionValue
    {
        public string Name;
        public Quaternion Value;
    }

    [System.Serializable]
    public struct StringValue
    {
        public string Name;
        public string Value;
    }

    [System.Serializable]
    public struct TextSegment
    {
        public string prepend;
        public string text;
        public string append;
    }

    [System.Serializable]
    public struct Vector2Value
    {
        public string Name;
        public Vector2 Value;
    }

    [System.Serializable]
    public struct Vector3Value
    {
        public string Name;
        public Vector3 Value;
    }

}