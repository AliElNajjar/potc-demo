using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBackgroundImage : MonoBehaviour
{
    [Header("Background")]
    //can be array of sprites for more than one background in same map
    [SerializeField] private Sprite Demo;
    [SerializeField] private RuntimeAnimatorController DemoAnimationController;


    private SpriteRenderer _spriteRenderer;
    private Animator _sceneAnimator;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sceneAnimator = GetComponent<Animator>();

        if (BattleData.Instance.backgroundAreaToLoad == Area.Demo1)
        {
            _spriteRenderer.sprite = Demo;
            _sceneAnimator.runtimeAnimatorController = DemoAnimationController;
        }
        else if (BattleData.Instance.backgroundAreaToLoad == Area.Demo2)
        {
            _spriteRenderer.sprite = Demo;
            _sceneAnimator.runtimeAnimatorController = DemoAnimationController;
        }
        else
        {
            //If something is wrong
            Debug.LogWarning("Unexpected result.");
            _spriteRenderer.sprite = Demo;
            _sceneAnimator.runtimeAnimatorController = DemoAnimationController;
        }
    }
}
