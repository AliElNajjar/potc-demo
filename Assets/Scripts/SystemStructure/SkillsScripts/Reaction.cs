using UnityEngine;

[CreateAssetMenu(fileName = "ReactionSkill", menuName = "Unit/Skills/Create Reaction Skill", order = 1)]
public class Reaction : Skill
{
    public override void ActivateSkill(UnitParameters unitToApply)
    {
        
    }

    public override string SkillType()
    {
        return nameof(Reaction);
    }
}