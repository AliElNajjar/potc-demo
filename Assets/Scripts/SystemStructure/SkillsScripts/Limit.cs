using UnityEngine;

[CreateAssetMenu(fileName = "LimitSkill", menuName = "Unit/Skills/Create Limit Skill", order = 1)]
public class Limit : Skill
{
    public override void ActivateSkill(UnitParameters unitToApply)
    {
        
    }

    public override string SkillType()
    {
        return nameof(Limit);
    }
}