using UnityEngine;

namespace NullSave.TOCK.Character
{
    public class CharacterCogPlugin : ScriptableObject
    {

        #region Properties

        public CharacterCog Character { get; set; }

        public Transform transform { get { return Character.transform; } }

        #endregion

        #region Public Methods

        public virtual void DrawGizmos(Transform host) { }

        public virtual void Initialize() { }

        public virtual void Movement() { }

        public virtual void OnUpdate() { }

        public virtual void PreMovement() { }

        public virtual void PostMovement() { }

        #endregion

    }
}