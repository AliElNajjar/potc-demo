using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkill", menuName = "Unit/Skills/Create Active", order = 1)]
public class Active : Skill
{
    public override void ActivateSkill(UnitParameters unitToApply)
    {
        
    }

    public override string SkillType()
    {
       return nameof(Active);
    }
}