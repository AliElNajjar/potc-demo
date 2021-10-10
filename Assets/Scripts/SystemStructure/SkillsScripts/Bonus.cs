using UnityEngine;

[CreateAssetMenu(fileName = "BonusSkill", menuName = "Unit/Skills/Create Bonus Skill", order = 1)]
public class Bonus : Skill
{
    public override void ActivateSkill(UnitParameters unitToApply)
    {
        
    }
    
    public override string SkillType()
    {
        return nameof(Bonus);
    }
}