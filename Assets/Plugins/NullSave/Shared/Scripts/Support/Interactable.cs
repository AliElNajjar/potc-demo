using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    [HierarchyIcon("touchbutton", false)]
    public class Interactable : MonoBehaviour
    {

        #region Variables

        public string displayText = "Interact";
        public bool directional = false;
        [Range(0, 1)] public float directionTolerance = 0.3f;

        public UnityEvent onInteract, onInteractLeft, onInteractRight, onInteractFront, onInteractBack;

        #endregion

        #region Public Methods

        public void Interact(Vector3 sourcePos)
        {
            if (!directional)
            {
                onInteract?.Invoke();
                return;
            }

            BasicDirection dir = GetDirection(sourcePos);
            switch (dir)
            {
                case BasicDirection.Back:
                    onInteractBack?.Invoke();
                    break;
                case BasicDirection.Front:
                    onInteractFront?.Invoke();
                    break;
                case BasicDirection.Left:
                    onInteractLeft?.Invoke();
                    break;
                case BasicDirection.Right:
                    onInteractRight?.Invoke();
                    break;
            }
        }

        #endregion

        #region Private Methods

        private BasicDirection GetDirection(Vector3 OtherObject)
        {
            Vector3 lPos = transform.position;
            lPos.y = OtherObject.y;
            Vector3 dir = (OtherObject - lPos).normalized;

            float forward = Vector3.Dot(dir, transform.forward);
            float right = Vector3.Dot(dir, transform.right);

            float adj = 1 - Mathf.Abs(right);
            if (right < 0 && 1 + right <= directionTolerance) return BasicDirection.Left;
            if (right > 0 && 1 - right <= directionTolerance) return BasicDirection.Right;

            if (forward >= 0)
            {
                return BasicDirection.Front;
            }
            return BasicDirection.Back;
        }

        #endregion

    }
}