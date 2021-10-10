using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace NullSave.TOCK
{
    public static class IOExtensions
    {

        #region Public Methods

        public static bool ReadBool(this Stream fs)
        {
            if (fs.ReadByte() > 0) return true;
            return false;
        }

        public static Color ReadColor(this Stream fs)
        {
            return new Color(fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat());
        }

        public static int ReadInt(this Stream fs)
        {
            byte[] b = new byte[4];
            fs.Read(b, 0, b.Length);
            return BitConverter.ToInt32(b, 0);
        }

        public static float ReadFloat(this Stream fs)
        {
            byte[] b = new byte[4];
            fs.Read(b, 0, b.Length);
            return BitConverter.ToSingle(b, 0);
        }

        public static long ReadLong(this Stream fs)
        {
            byte[] b = new byte[8];
            fs.Read(b, 0, b.Length);
            return BitConverter.ToInt64(b, 0);
        }

        public static Quaternion ReadQuaternion(this Stream fs)
        {
            return new Quaternion(fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat());
        }

        public static Rect ReadRect(this Stream fs)
        {
            return new Rect(fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat());
        }

        public static string ReadStringPacket(this Stream fs)
        {
            byte[] b = new byte[fs.ReadLong()];
            fs.Read(b, 0, b.Length);
            return new string(Encoding.UTF32.GetChars(b));
        }

        public static Vector2 ReadVector2(this Stream fs)
        {
            return new Vector2(fs.ReadFloat(), fs.ReadFloat());
        }

        public static Vector3 ReadVector3(this Stream fs)
        {
            return new Vector3(fs.ReadFloat(), fs.ReadFloat(), fs.ReadFloat());
        }

        public static void WriteBool(this Stream fs, bool value)
        {
            if (value)
            {
                fs.WriteByte(1);
            }
            else
            {
                fs.WriteByte(0);
            }
        }

        public static void WriteColor(this Stream fs, Color value)
        {
            fs.WriteFloat(value.r);
            fs.WriteFloat(value.g);
            fs.WriteFloat(value.b);
            fs.WriteFloat(value.a);
        }

        public static void WriteInt(this Stream fs, int value)
        {
            byte[] b = BitConverter.GetBytes(value);
            fs.Write(b, 0, b.Length);
        }

        public static void WriteFloat(this Stream fs, float value)
        {
            byte[] b = BitConverter.GetBytes(value);
            fs.Write(b, 0, b.Length);
        }

        public static void WriteLong(this Stream fs, long value)
        {
            byte[] b = BitConverter.GetBytes(value);
            fs.Write(b, 0, b.Length);
        }

        public static void WriteQuaternion(this Stream fs, Quaternion value)
        {
            fs.WriteFloat(value.x);
            fs.WriteFloat(value.y);
            fs.WriteFloat(value.z);
            fs.WriteFloat(value.w);
        }

        public static void WriteRect(this Stream fs, Rect rect)
        {
            fs.WriteFloat(rect.x);
            fs.WriteFloat(rect.y);
            fs.WriteFloat(rect.width);
            fs.WriteFloat(rect.height);
        }

        public static void WriteStringPacket(this Stream fs, string value)
        {
            byte[] b = Encoding.UTF32.GetBytes(value);
            fs.WriteLong(b.Length);
            fs.Write(b, 0, b.Length);
        }

        public static void WriteVector2(this Stream fs, Vector2 value)
        {
            fs.WriteFloat(value.x);
            fs.WriteFloat(value.y);
        }

        public static void WriteVector3(this Stream fs, Vector3 value)
        {
            fs.WriteFloat(value.x);
            fs.WriteFloat(value.y);
            fs.WriteFloat(value.z);
        }

        #endregion

    }
}