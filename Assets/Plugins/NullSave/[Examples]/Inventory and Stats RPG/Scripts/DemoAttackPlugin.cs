using NullSave.TOCK.Character;
using UnityEngine;

[CreateAssetMenu(menuName = "TOCK/Inventory/(Demo) Attack Plugin")]
public class DemoAttackPlugin : CharacterCogPlugin
{

    #region Variables

    public string attackButton = "Fire1";
    public bool allowRetrigger = false;
    public float extraStopMoveTime = 0.2f;

    private float preventMoveRemain;

    #endregion

    #region Plugin Methods

    public override void OnUpdate()
    {
        if (preventMoveRemain > 0 && !Character.IsAttacking)
        {
            preventMoveRemain -= Time.deltaTime;
        }

        if (Character.PreventButtonUpdate || Character.InAction) return;

        if ((!Character.IsAttacking || allowRetrigger) && Character.GetButtonDown(attackButton) && Character.Animator.GetBool("IsHoldingWeapon"))
        {
            Character.IsAttacking = true;
            Character.Animator.SetTrigger("Attack");
            preventMoveRemain = extraStopMoveTime;
        }
    }

    public override void PreMovement()
    {
        if (preventMoveRemain > 0)
        {
            Character.PreventMovement = true;
        }
    }

    #endregion

}
