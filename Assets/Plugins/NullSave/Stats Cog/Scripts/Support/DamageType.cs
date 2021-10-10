using System.IO;
using UnityEngine;

namespace NullSave.TOCK
{
    [CreateAssetMenu(menuName = "TOCK/Combat/Damage Type", order = 1)]
    public class DamageType : ScriptableObject
    {

        #region Variables

        public string displayName;
        public Sprite icon;

        #endregion

        #region Public Methods

        public static DamageType Load(Stream stream)
        {
            DamageType resVal = CreateInstance<DamageType>();
            resVal.displayName = stream.ReadStringPacket();
            byte[] spriteData = new byte[stream.ReadInt()];
            if (spriteData.Length > 0)
            {
                stream.Read(spriteData, 0, spriteData.Length);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(spriteData);
                resVal.icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            return resVal;
        }

        public void Save(Stream stream)
        {
            stream.WriteStringPacket(displayName);
            if (icon == null)
            {
                stream.WriteInt(0);
            }
            else
            {
                byte[] spriteData = icon.texture.EncodeToPNG();
                stream.WriteInt(spriteData.Length);
                stream.Write(spriteData, 0, spriteData.Length);
            }
        }

        #endregion

    }
}