using Scripts.Managers;
using Scripts.UI;
using System.Collections;
using UnityEngine;


public class UnitOverWorldMovement : MonoBehaviour
{
    public AnimationData unitAnimationData;
    [Range(0.01f, 0.20f)] public float translateUnit = 0.01f;
    public byte _runMultiplier = 2;
    private static bool paused, inBattle;
    private bool keysDown, readingInput;
    public bool isNPC = false;
    [ReadOnly] public bool moving;
    public float interactRadius = 0.365f;
    public SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rbody;

    private SimpleAnimator animator;
    private Vector3 defaultScale;

    public bool ReadingMovement
    {
        get;
        set;
    }

    #region Animation Constants
    const string kIdleLeftAnim = "IdleLeft";
    const string kIdleRightAnim = "IdleRight";
    const string kIdleUpAnim = "IdleUp";
    const string kIdleDownAnim = "IdleDown";
    const string kRunRightAnim = "RunRight";
    const string kRunLeftAnim = "RunLeft";
    const string kRunDownAnim = "RunDown";
    const string kRunUpAnim = "RunUp";
    const string kWalkLeftAnim = "WalkLeft";
    const string kWalkRightAnim = "WalkRight";
    const string kWalkUpAnim = "WalkUp";
    const string kWalkDownAnim = "WalkDown";
    const string kTalkSideAnim = "TalkSide";
    const string kTalkDownAnim = "TalkDown";
    const string kTalkUpAnim = "TalkUp";
    //const string kTauntAnim = "Taunt";
    #endregion


    private Vector3 _movement = Vector3.zero;
    private float _multiplier = 1;
    public float _moveUnitsPerSecondScale = 200.0f;

    [SerializeField, ReadOnly] private State state;
    [SerializeField, ReadOnly] private Looking _lookingDir = Looking.Down;

    public bool running;

    public static int loadPosition = 0;
    //private bool movingFromFFAScene = false;

    public Vector3 MovementValue
    {
        get { return _movement; }
        set { _movement = value; }
    }

    public Looking LookingDir
    {
        get { return _lookingDir; }
        set { _lookingDir = value; }
    }

    private void Awake()
    {
        animator = GetComponent<SimpleAnimator>();
        if (unitAnimationData) unitAnimationData.ApplyAnimations(ref animator);

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rbody = GetComponent<Rigidbody2D>();

        defaultScale = transform.localScale;

        GameManager.Instance?.OnGameStateChanged.AddListener(HandleGameStateChanged);
        GameManager.Instance?.OnLevelStateChanged.AddListener(HandleLevelStateChanged);

    }

    private void Start()
    {  
        if (!isNPC)
        {
            if(MessageManager.Instance != null) MessageManager.Instance.OnMessageEnd += EnableMovement;

            if (GameManager.startingNewGame || GameManager.loadingFromEvent)
            {
                loadPosition = 0;
                GameManager.startingNewGame = false;
                GameManager.loadingFromEvent = false;
            }

            GameObject[] loadPositions = GameObject.FindGameObjectsWithTag("LoadTrigger");

            for (int i = 0; i < loadPositions.Length; i++)
            {   
                if (loadPositions[i].GetComponent<SceneLoadTrigger>().loadPosition == loadPosition)
                {
                    transform.position = loadPositions[i].transform.GetChild(0).transform.position;//spawn point pos.
                    loadPosition = -1;
                    break;
                }
            }

            ReadingMovement = true;
            readingInput = true;
        }
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        paused = (currentState == GameManager.GameState.PAUSED);
    }

    void HandleLevelStateChanged(GameManager.LevelState currentLevelState)
    {
        inBattle = (currentLevelState == GameManager.LevelState.BATTLE);
    }

    private void Update()
    {   
        // player won't get inputs when game is paused but pausing doesn't affect battle state inputs.
        if (!paused || inBattle)
        {
            
            if (!isNPC) //later when we get some NPC logic
            {
                Inputs();
                CheckForInteractables();
                UpdatePlayerState();
            }

            if (isNPC)
            {
                Movement();
            }
        }
    }

    private void Inputs()
    {
        if (!readingInput) return;
        
        //GameManager.Instance.PlayerIsMoving();
        if (RewiredInputHandler.Instance.player.GetButtonDown("Run"))
        {
            running = true;
        }
        else if (RewiredInputHandler.Instance.player.GetButtonUp("Run"))
        {
            running = false;
        }

        bool down = (RewiredInputHandler.Instance.player.GetButton("Down"));
        bool up = (RewiredInputHandler.Instance.player.GetButton("Up"));
        bool right = (RewiredInputHandler.Instance.player.GetButton("Right"));
        bool left = (RewiredInputHandler.Instance.player.GetButton("Left"));

        keysDown = (down || up || right || left);
        
        if (down)
        {
            _movement = Vector2.down;
            _lookingDir = Looking.Down;

            //For diagonal movement
            if (left)
            {
                _movement = new Vector2(-1, -1);
                //need sprite for diagonal movement
            }

            else if (right)
            {
                _movement = new Vector2(1, -1);
            }
        }
        else if (up)
        {
            _movement = Vector2.up;
            _lookingDir = Looking.Up;

            //For diagonal movement
            if (left)
            {
                _movement = new Vector2(-1, 1);
            }

            else if (right)
            {
                _movement = new Vector2(1, 1);
            }
        }
        else if (right)
        {
            _movement = Vector2.right;
            _lookingDir = Looking.Right;

            //For diagonal movement
            if (up)
            {
                _movement = new Vector2(1, 1);
            }

            else if (down)
            {
                _movement = new Vector2(1, -1);
            }
        }
        else if (left)
        {
            _movement = Vector2.left;
            _lookingDir = Looking.Left;

            //For diagonal movement
            if (up)
            {
                _movement = new Vector2(-1, 1);
            }

            else if (down)
            {
                _movement = new Vector2(-1, -1);
            }
        }
        else
        {
            _movement = Vector2.zero; 
        }

        if (RewiredInputHandler.Instance.player.GetButtonUp("Down") ||
            RewiredInputHandler.Instance.player.GetButtonUp("Up") ||
            RewiredInputHandler.Instance.player.GetButtonUp("Left") ||
            RewiredInputHandler.Instance.player.GetButtonUp("Right"))
        {
            _movement = Vector2.zero;
            //moving = false;
            //_state = OverworldUnitState.Idle;
        }

        Movement();

        /*if (_state != OverworldUnitState.Idle)
        {
            _horizontal = _movement.x;
            _vertical = _movement.y;
        }*/
    }

    private void UpdatePlayerState()
    {
        Events.PlayerMoving?.Invoke(moving);
    }

    private void Movement()
    {
        if (!ReadingMovement) return;
        Translate(_movement);
    }

    public void Translate(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            moving = true;

            float movementTypeMultiplier = _moveUnitsPerSecondScale * (running ? _runMultiplier : _multiplier);

            Vector3 velocity = direction * translateUnit * movementTypeMultiplier;
            _rbody.velocity = velocity;

            if (running)
            {
                switch (_lookingDir)
                {
                    case Looking.Right:
                        SetOrKeepState(State.RunRight);
                        break;
                    case Looking.Left:
                        SetOrKeepState(State.RunLeft);
                        break;
                    case Looking.Up:
                        SetOrKeepState(State.RunUp);
                        break;
                    case Looking.Down:
                        SetOrKeepState(State.RunDown);
                        break;
                }
            }
            else if (!running)
            {
                switch (_lookingDir)
                {
                    case Looking.Right:
                        SetOrKeepState(State.WalkRight);
                        break;
                    case Looking.Left:
                        SetOrKeepState(State.WalkLeft);
                        break;
                    case Looking.Up:
                        SetOrKeepState(State.WalkUp);
                        break;
                    case Looking.Down:
                        SetOrKeepState(State.WalkDown);
                        break;
                }
            }
        }
        else
        {

            moving = false;
            _rbody.velocity = Vector3.zero;

            switch (_lookingDir)
            {
                case Looking.Right:
                    SetOrKeepState(State.IdleSide_Right);
                    break;
                case Looking.Left:
                    SetOrKeepState(State.IdleSide_Left);
                    break;
                case Looking.Up:
                    SetOrKeepState(State.IdleUp);
                    break;
                case Looking.Down:
                    SetOrKeepState(State.IdleSide_Right);
                    break;
            }
        }
    }

    public void LookInDirIdle_Force(Looking lookDir)
    {
        switch (lookDir)
        {
            case Looking.Right:
                SetOrKeepState(State.IdleSide_Right);
                break;
            case Looking.Left:
                SetOrKeepState(State.IdleSide_Right);
                break;
            case Looking.Up:
                SetOrKeepState(State.IdleUp);
                break;
            case Looking.Down:
                SetOrKeepState(State.IdleSide_Right);
                break;
        }
    }

    public IEnumerator PlayFootsteps()
    {
        if (!GetComponent<AudioSource>())
            yield break;

        AudioSource audioSource = GetComponent<AudioSource>();

        while (true)
        {
            if (_rbody.velocity.magnitude > 0)
            {
                if (!audioSource.isPlaying)
                {
                    if (Time.frameCount % 5 == 0) audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            yield return null;
        }
    }

    public void ToggleMovement(bool enable)
    {
        if (enable) EnableMovement();
        if (!enable) DisableMovement();
    }

    private void DisableMovement()
    {
        ReadingMovement = false;
        animator.Stop();
        //_state = OverworldUnitState.Idle;

    }

    private void EnableMovement()
    {
        ReadingMovement = true;
        animator.Resume();
    }

    public void DisableInputs()
    {
        readingInput = false;
    }

    public void EnableInputs()
    {
        readingInput = true;
    }

    private void CheckForInteractables()
    {
       Vector3 startingPos = transform.position;

        var colliders = Physics2D.OverlapCircleAll(startingPos, interactRadius);

        if (colliders != null)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<Interactable>())
                {
                    var interactable = collider.gameObject.GetComponent<Interactable>();

                    if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
                    {

                        DisableMovement();
                        interactable.ActivateInteractable();

                        break; //don't try to do multiple interactions at once so stop after first valid one found
                    }
                }
            }

        }
    }

    public static Vector3 GetDirection(Looking lookingDirection)
    {
        switch (lookingDirection)
        {
            case Looking.Right:
                return Vector3.right;
            case Looking.Left:
                return Vector3.left;
            case Looking.Up:
                return Vector3.up;
            case Looking.Down:
                return Vector3.down/* * 2.32121212f*/; //account for the pivot being near the face, meaning above the character there's a lot of gap right above the transform position, but on bottom the gap is a ways below the transform position
            default:
                return Vector3.zero;
        }
    }

    //private static State GetWalkingState(Looking lookingDirection)
    //{
    //    switch (lookingDirection)
    //    {
    //        case Looking.Up:
    //            return State.WalkUp;
    //        case Looking.Down:
    //            return State.WalkDown;
    //        default:
    //            return State.WalkSide;
    //    }
    //}

    public static State GetIdleState(Looking lookingDirection)
    {
        return lookingDirection switch
        {
            Looking.Left => State.IdleSide_Left,
            Looking.Right => State.IdleSide_Right,
            Looking.Up => State.IdleUp,
            Looking.Down => State.IdleDown,
            _ => State.IdleSide_Left,
        };
    }


    public void SetOrKeepState(State state)
    {
        if (this.state == state) return;
        EnterState(state);
    }

    void ExitState()
    {

    }

    private void EnterState(State state)
    {
        ExitState();
        switch (state)
        {
            case State.IdleSide_Left:
                animator.Play(kIdleLeftAnim);
                break;
            case State.IdleSide_Right:
                animator.Play(kIdleRightAnim);
                break;
            case State.IdleUp:
                animator.Play(kIdleUpAnim);
                break;
            case State.IdleDown:
                animator.Play(kIdleDownAnim);
                break;
            case State.RunLeft:
                animator.Play(kRunLeftAnim);
                break;
            case State.RunRight:
                animator.Play(kRunRightAnim);
                break;
            case State.RunUp:
                animator.Play(kRunUpAnim);
                break;
            case State.RunDown:
                animator.Play(kRunDownAnim);
                break;
            case State.WalkLeft:
                animator.Play(kWalkLeftAnim);
                break;
            case State.WalkRight:
                animator.Play(kWalkRightAnim);
                break;
            case State.WalkUp:
                animator.Play(kWalkUpAnim);
                break;
            case State.WalkDown:
                animator.Play(kWalkDownAnim);
                break;
        }

        this.state = state;
    }


    //private void Face(int direction)
    //{
    //    if (direction == 1 || direction == -1)
    //        transform.localScale = new Vector3(defaultScale.x * direction, defaultScale.y, defaultScale.z);
    //}

    public Vector3 GetDir(Looking dir)
    {
        switch (dir)
        {
            case Looking.Right:
                return Vector3.right;
            case Looking.Left:
                return Vector3.left;
            case Looking.Up:
                return Vector3.up;
            case Looking.Down:
                return Vector3.down;
        }

        return Vector3.zero;
    }
}

public enum Looking
{
    Right = 1,
    Left = -1,
    Up = 10,
    Down = -10
}

public enum State
{
    IdleSide_Left = 0,
    IdleSide_Right = 1,
    IdleUp = 2,
    IdleDown = 3,
    RunLeft = 4,
    RunRight = 5,
    RunUp = 6,
    RunDown = 7,
    WalkLeft = 8,
    WalkRight = 9,
    WalkUp = 10,
    WalkDown = 11
}