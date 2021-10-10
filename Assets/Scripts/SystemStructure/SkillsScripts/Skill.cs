using UnityEngine;


/* skill x 
 * focused
 * wide
 * physical 
 * magical 
 * 
 * 
 */
/*
 *  There will be 10 slots total a character has skills in:
 *  the four active skills, which are what they select to use (activate) in battle,
 *  like moves in Pokemon,
 *  their one unique passive skill and two teachable passive skills,
 *  which are like abilities in Pokemon or feats in DnD, then they have two slots to equip reactions to, 
 *  and one slot for their limit.
I'll clean up the list of skills some and try to get you a sample from that within the next couple of hours.
 */
/*
 *  Any skill learned from any style can be equipped, but if they do not have the necessary weapon or armor requirements
 *  fulfilled to use them, they will be grayed out.
 *  This awards players who play around with different styles for their units,
 *  but also doesn’t too heavily punish players who aren’t interested in min/maxing builds.
 */

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected UnitParameters skillParameters;
    [SerializeField] protected Targeting targeting;
    [SerializeField] protected SkillTier skillTier;
    [SerializeField] protected SkillLevel skillLevel;
    [SerializeField] protected SkillElement skillElement;
    [SerializeField] protected float power;
    [SerializeField] protected float range;
    [SerializeField] protected float MPCost;

    public abstract void ActivateSkill(UnitParameters unitToApply);

    public abstract string SkillType();
}

public enum Targeting
{
    Focused = 0,
    Wide = 1
}

public enum SkillTier
{
    E = 0,
    D = 1,
    C = 2,
    B = 3,
    A = 4,
    S = 5
}

public enum SkillLevel
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8
}

public enum SkillElement
{
    Light = 0,
    Wind = 1,
    Ice = 2,
    Water = 3,
    Earth = 4,
    Fire = 5,
    Thunder = 6,
    Dark = 7,
    None = 8
}