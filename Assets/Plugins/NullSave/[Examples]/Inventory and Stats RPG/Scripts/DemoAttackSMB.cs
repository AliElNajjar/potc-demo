using NullSave.TOCK.Character;
using UnityEngine;

public class DemoAttackSMB : StateMachineBehaviour
{

    #region Variables

    [Tooltip("normalizedTime of Active Damage")]
    public float startDamage = 0.05f;
    [Tooltip("normalizedTime of Disable Damage")]
    public float endDamage = 0.9f;

    private bool isActive;

    #endregion

    /// <summary>
    /// Release attack lock
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animatorStateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.GetComponent<CharacterCog>().IsAttacking = false;
        animator.GetComponent<CharacterCog>().SetDamageDealersActive(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 1f) return;
        if (!isActive && stateInfo.normalizedTime % 1 >= startDamage && stateInfo.normalizedTime % 1 <= endDamage)
        {
            animator.GetComponent<CharacterCog>().SetDamageDealersActive(true);
            isActive = true;
        }
        else if (stateInfo.normalizedTime % 1 >= endDamage && isActive)
        {
            animator.GetComponent<CharacterCog>().SetDamageDealersActive(false);
            isActive = false;
        }
    }

}