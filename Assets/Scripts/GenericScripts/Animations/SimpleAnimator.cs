using UnityEngine;

using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class SimpleAnimator : MonoBehaviour
{
    #region Public Properties
    [System.Serializable]
    public class Anim
    {
        public string name;
        public Sprite[] frames;
        public float framesPerSec = 5;
        public bool loop = true;

        public float duration
        {
            get
            {
                return frames.Length * framesPerSec;
            }
            set
            {
                framesPerSec = value / frames.Length;
            }
        }

        public int Hash
        {
            get { return Animator.StringToHash(name); }
        }
    }
    public List<Anim> animations = new List<Anim>();

    [HideInInspector]
    public int currentFrame;

    [HideInInspector]
    public bool IsDone
    {
        get { return currentFrame >= current.frames.Length; }
    }

    [HideInInspector]
    public bool IsPlaying
    {
        get { return _playing; }
    }

    #endregion
    //--------------------------------------------------------------------------------
    #region Private Properties
    SpriteRenderer spriteRenderer;
    Anim current;
    bool _playing;
    float secsPerFrame;
    float nextFrameTime;

    #endregion
    //--------------------------------------------------------------------------------
    #region Editor Support
    [ContextMenu("Sort All Frames by Name")]
    void DoSort()
    {
        foreach (Anim anim in animations)
        {
            System.Array.Sort(anim.frames, (a, b) => a.name.CompareTo(b.name));
        }
        Debug.Log(gameObject.name + " animation frames have been sorted alphabetically.");
    }
    #endregion
    //--------------------------------------------------------------------------------
    #region MonoBehaviour Events
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log(gameObject.name + ": Couldn't find SpriteRenderer");
        }

        if (animations.Count > 0) PlayByIndex(0);
    }

    void Update()
    {
        if (!_playing || Time.time < nextFrameTime || spriteRenderer == null) return;
        currentFrame++;
        if (currentFrame >= current.frames.Length)
        {
            if (!current.loop)
            {
                _playing = false;
                return;
            }
            currentFrame = 0;
        }
        spriteRenderer.sprite = current.frames[currentFrame];
        nextFrameTime += secsPerFrame;
    }

    #endregion
    //--------------------------------------------------------------------------------
    #region Public Methods
    public void Play(string name/*, bool idle = false*/)
    {
        int index = animations.FindIndex(a => a.name == name);

        /*if (idle && transform.GetComponent<UnitOverworldMovement>().isNPC)
        {
            
            transform.GetComponent<NPCBehavior>().isStatic = true;
            transform.GetComponent<NPCBehavior>().walkingAround = false;
            transform.GetComponent<NPCBehavior>().isMoving = false;

        }*/

        if (index < 0)
        {
            Debug.LogError(gameObject + ": No such animation: " + name);
        }
        else
        {
            PlayByIndex(index);
        }

    }

    public void Play(int nameHash)
    {
        int index = animations.FindIndex(a => a.Hash == nameHash);
        if (index < 0)
        {
            Debug.LogError(gameObject + ": No such animation: " + name);
        }
        else
        {
            PlayByIndex(index);
        }
    }

    public void PlayByIndex(int index)
    {
        if (index < 0) return;
        Anim anim = animations[index];

        current = anim;

        secsPerFrame = 1f / anim.framesPerSec;
        currentFrame = -1;
        _playing = true;
        nextFrameTime = Time.time;


    }

    public void Stop()
    {
        _playing = false;
    }

    public void Resume()
    {
        _playing = true;
        nextFrameTime = Time.time + secsPerFrame;
    }

    #endregion
    //--------------------------------------------------------------------------------
    #region Animation After Interaction
    /*public string GetAnimationName(Looking direction, bool idle = false)
    {
        if (idle)
        {
            switch (direction)
            {
                case Looking.Right:
                    return "IdleSide";
                case Looking.Left:
                    return "IdleSide";
                case Looking.Up:
                    return "IdleUp";
                case Looking.Down:
                    return "IdleDown";
                default:
                    return "No Animation Found";
            }
        }

        switch (direction)
        {
            case Looking.Right:
                return "ChestPoundSide";
            case Looking.Left:
                return "ChestPoundSide";
            case Looking.Up:
                return "ChestPoundUp";
            case Looking.Down:
                return "ChestPoundDown";
            default:
                return "No Animation Found";
        }
    }
    public void AfterInteractAnimation()
    {
        var direction = transform.GetComponent<UnitOverworldMovement>().LookingDir;
        string animationName = GetAnimationName(direction);
        string idleAnimationName = GetAnimationName(direction, true);
        StartCoroutine(StartAnimation(animationName, 2f, idleAnimationName));
    }

    public IEnumerator StartAnimation(string animationName, float waitTime, string idleAnimationName)
    {

        yield return null;
        Play(animationName);

        yield return new WaitForSeconds(waitTime);

        Play(idleAnimationName);
        //transform.GetComponent<NPCBehavior>().isStatic = false;

    }*/
    #endregion
}






