using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animations/AnimationData", fileName = "UnitAnimationData"), System.Serializable]
public class AnimationData : ScriptableObject
{
    public List<SimpleAnimator.Anim> animations;

    public void ApplyAnimations(ref SimpleAnimator simpleAnimator)
    {
        simpleAnimator.animations = this.animations;
    }
}

[System.Serializable]
public class SpriteAnimationPairs
{
    public string animationName;
    public int spriteNum;
}
