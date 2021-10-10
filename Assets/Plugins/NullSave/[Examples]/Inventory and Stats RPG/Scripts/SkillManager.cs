using NullSave.TOCK.Inventory;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    public Animator playerAnimator;

    public void UseSkill(SkillSlotUI skillSlot)
    {
        if (skillSlot.AttachedSkillItem == null) return;

        switch(skillSlot.AttachedSkillItem.name)
        {
            case "Feather of Lightness":
                playerAnimator.SetTrigger("Jump");
                break;
        }

    }

}
