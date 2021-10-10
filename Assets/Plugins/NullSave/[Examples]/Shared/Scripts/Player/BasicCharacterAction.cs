using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(BasicPersonController))]
    public class BasicCharacterAction : MonoBehaviour
    {

        #region Variables

        public KeyCode key = KeyCode.None;
        public string triggerName;
        public string waitWhileInState;

#if STATS_COG

        public bool requireExpression;
        public string expression;

#endif

        public UnityEvent onActionStart, onActionEnd;

        private bool inAction;
        private Animator animator;
        private BasicPersonController basicCharacter;
        private float minTime;
               
        #endregion

        #region Unity Methods

        private void Awake()
        {
            animator = GetComponent<Animator>();
            basicCharacter = GetComponent<BasicPersonController>();
        }

        private void Update()
        {
            if (Time.timeScale == 0) return;

            if (inAction)
            {
                if (Time.time >= minTime && !animator.GetCurrentAnimatorStateInfo(0).IsName(waitWhileInState))
                {
                    inAction = false;
                    basicCharacter.LockInput = false;
                    onActionEnd?.Invoke();
                }
            }
            else
            {
                if (!basicCharacter.LockInput && Input.GetKeyDown(key))
                {
#if STATS_COG
                    if (requireExpression && !GetComponent<Stats.StatsCog>().EvaluateCondition(expression))
                    {
                        Debug.Log("Expression failed: " + expression);
                        return;
                    }
#endif
                    minTime = Time.time + 0.3f;
                    inAction = true;
                    basicCharacter.LockInput = true;
                    animator.SetTrigger(triggerName);
                    onActionStart?.Invoke();
                }
            }
        }

        #endregion

    }
}