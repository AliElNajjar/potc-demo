using UnityEngine;

namespace NullSave.TOCK
{

    [System.Serializable]
    public class Padding
    {

        #region Variables

        public float left;
        public float top;
        public float right;
        public float bottom;

        #endregion

        #region Constructors

        public Padding() { }

        public Padding(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        #endregion

        #region Properties

        public float Horizontal { get { return left + right; } }

        public float Vertical { get { return top + bottom; } }

        #endregion

        #region Public Methods

        public Vector2 ToVector2()
        {
            return new Vector2(left + right, top + bottom);
        }

        #endregion

    }

}