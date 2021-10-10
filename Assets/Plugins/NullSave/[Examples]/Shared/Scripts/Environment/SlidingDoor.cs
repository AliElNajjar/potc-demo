using UnityEngine;

namespace NullSave.TOCK
{
    public class SlidingDoor : MonoBehaviour
    {

        #region Variables

        public bool unlocked = true;

        private Animator animator;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!unlocked) return;
            animator.SetBool("Opened", true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!unlocked) return;
            animator.SetBool("Opened", false);
        }

        #endregion

    }
}