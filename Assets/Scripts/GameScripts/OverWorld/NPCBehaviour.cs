using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    public Collider2D bounds;

    public float walkSpeed = 0.05f;

    public bool walkingAround;
    [Range(0.5f, 10f)]
    public float m_walkAroundInterval = 2f;
    public bool isStatic;
    public bool canAttack = false;

    [Header("NPC Rotation"), Space]
    public bool idleRotation = false;
    [Tooltip("This variable means which every x seconds player will rotate.It is related with idleRotate variable.")]
    [Range(0.5f, 10f)]
    public float idleRotationTime = 1f;
    public bool m_rotateHorizontal = true, m_rotateVerticle = true;

    [ReadOnly] public bool isMoving;
    [ReadOnly] public bool isChatting = false;

    private UnitOverWorldMovement _unitMovement;

    private Vector3 _minBounds;
    private Vector3 _maxBounds;
    private List<Vector3> walkPositions = new List<Vector3>();

    private int currentWalkPosIndex = 0;
    private int nextPos = 1;

    [HideInInspector] public Vector3 nextPosition;

    [HideInInspector] public List<Vector3> defaultPositions;
    private Vector3 vDirection;
    private bool trigger = false;
    private bool chatStartByPlayer = false;
    private Coroutine walkAroundRoutine, rotatingRoutine;

    private void Start()
    {
        vDirection = new Vector3();
        defaultPositions = new List<Vector3>();
        nextPosition = new Vector3();

        _unitMovement = GetComponent<UnitOverWorldMovement>();
        _unitMovement.ReadingMovement = true;

        walkPositions = new List<Vector3>();
        walkPositions.Add(transform.position);
        defaultPositions.Add(transform.position);
        if (bounds == null)
        {
            foreach (Transform pos in this.transform)
            {
                if (pos.CompareTag("Waypoint"))
                {
                    walkPositions.Add(pos.position);
                    defaultPositions.Add(pos.position);
                }
                else if (pos.name.Equals("Initial LookingDir"))
                {
                    Vector3 dir = pos.position - transform.position;

                    _unitMovement.LookingDir = GetLookingDir2(dir);
                    Debug.Log("Initial looking direction found. Initial looking dir: " + _unitMovement.LookingDir);

                    State npcState = UnitOverWorldMovement.GetIdleState(_unitMovement.LookingDir);
                    Debug.Log("NPC initial state desired: " + npcState.ToString());
                    _unitMovement.SetOrKeepState(State.IdleSide_Right);
                    _unitMovement.SetOrKeepState(npcState);
                }
            }
        }
        else
        {
            walkPositions.Add(new Vector3(bounds.bounds.min.x, bounds.bounds.min.y));
            walkPositions.Add(new Vector3(bounds.bounds.min.x, bounds.bounds.max.y));
            walkPositions.Add(new Vector3(bounds.bounds.max.x, bounds.bounds.max.y));
            walkPositions.Add(new Vector3(bounds.bounds.max.x, bounds.bounds.min.y));
        }

        if (walkPositions.Count == 0 && !isStatic)
        {
            Debug.Log("Changing NPC behavior to static since no walk positions found.");
            isStatic = true;
        }

        if (isStatic)
        {
            walkingAround = false;
        }

        if (bounds != null)
        {
            _minBounds = bounds.bounds.min;
            _maxBounds = bounds.bounds.max;
            bounds.enabled = false;
        }

        if (idleRotation)
        {
            StartIdleRotation();
        }



        StartWalkAround();
    }

    /// <summary>
    /// Choose random direction will change the looking direction of the player randomly
    /// </summary>

    private List<Looking> m_allLookDirections = new List<Looking>();
    IEnumerator ChooseRandomDirection()
    {
        WaitForSeconds wfs = new WaitForSeconds(idleRotationTime);
        WaitForSeconds wfsX2 = new WaitForSeconds(idleRotationTime * 3);
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (!_unitMovement.moving && (m_rotateHorizontal || m_rotateVerticle) && !isChatting)
            {
                m_allLookDirections.Clear();

                if (m_rotateHorizontal)
                {
                    m_allLookDirections.Add(Looking.Left);
                    m_allLookDirections.Add(Looking.Right);
                }
                if (m_rotateVerticle)
                {
                    m_allLookDirections.Add(Looking.Up);
                    m_allLookDirections.Add(Looking.Down);
                }

                m_allLookDirections.Remove(_unitMovement.LookingDir);

                if (_unitMovement.ReadingMovement)
                {
                    _unitMovement.LookingDir = m_allLookDirections[Random.Range(0, m_allLookDirections.Count)];
                }
                else
                {
                    _unitMovement.LookInDirIdle_Force(m_allLookDirections[Random.Range(0, m_allLookDirections.Count)]);
                }
            }
            if (walkingAround)
            {
                yield return wfs;
            }
            else
            {
                yield return wfsX2;
            }
        }
    }

    private void StartIdleRotation()
    {
        rotatingRoutine = StartCoroutine(ChooseRandomDirection());
    }

    private void Update()
    {
        if (bounds != null)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, _minBounds.x, _maxBounds.x),
                Mathf.Clamp(transform.position.y, _minBounds.y, _maxBounds.y),
                transform.position.z
                );
        }
    }

    private void StartWalkAround()
    {
        if (walkPositions.Count <= 1)
        {
            return;
        }

        walkAroundRoutine = StartCoroutine(WalkAround());
    }

    private void StopWalkAround()
    {
        StopCoroutine(walkAroundRoutine);
        walkAroundRoutine = null;
    }

    private IEnumerator WalkAround()
    {
        nextPosition = GetCurrentPos();

        if (Vector3.Distance(transform.position, nextPosition) < 0.01f)
        {
            ReachedToCurrentPos();
        }

        while (walkingAround)
        {
            isMoving = true;
            trigger = true;

            nextPosition = GetCurrentPos();

            Looking direction = GetDirectionFromTwoPositions(transform.position, nextPosition);
            vDirection = UnitOverWorldMovement.GetDirection(direction);
            WalkInDirection(direction);

            float distanceBetweenPoints = Vector3.Distance(transform.position, nextPosition);
            float aux = distanceBetweenPoints;

            while (aux >= 0.01f)
            {
                if (aux > distanceBetweenPoints)
                    break;
                else
                    distanceBetweenPoints = aux;

                yield return null;

                aux = Vector3.Distance(transform.position, nextPosition);
            }

            SetEntityMovement(Vector3.zero);

            ReachedToCurrentPos();

            yield return new WaitForSeconds(m_walkAroundInterval);
        }
    }

    /// <summary>
    /// Move the NPC in the specified direction.
    /// </summary>
    /// <param name="dir"></param>
    private void WalkInDirection(Looking dir)
    {
        _unitMovement.LookingDir = dir;
        switch (dir)
        {
            case Looking.Right:
                SetEntityMovement(Vector3.right * walkSpeed);
                break;
            case Looking.Left:
                SetEntityMovement(Vector3.left * walkSpeed);
                break;
            case Looking.Up:
                SetEntityMovement(Vector3.up * walkSpeed);
                break;
            case Looking.Down:
                SetEntityMovement(Vector3.down * walkSpeed);
                break;
        }

    }

    public void StartConversation(Looking opposingDir)
    {
        isChatting = true;
        if (rotatingRoutine != null)
        {
            StopCoroutine(rotatingRoutine);
        }
        PauseBehavior();
        _unitMovement.ReadingMovement = false;
        LookAtPlayer(opposingDir);

        if (trigger)
        {
            chatStartByPlayer = true;
        }
    }

    public void LookAtPlayer(Looking opposingDir)
    {
        switch (opposingDir)
        {
            case Looking.Right:
                SetEntityLookingDirection(Vector3.left);
                _unitMovement.SetOrKeepState(State.IdleSide_Right);
                break;
            case Looking.Left:
                SetEntityLookingDirection(Vector3.right);
                _unitMovement.SetOrKeepState(State.IdleSide_Left);
                break;
            case Looking.Up:
                SetEntityLookingDirection(Vector3.down);
                _unitMovement.SetOrKeepState(State.IdleDown);
                break;
            case Looking.Down:
                SetEntityLookingDirection(Vector3.up);
                _unitMovement.SetOrKeepState(State.IdleUp);
                break;
        }
    }

    public void ResetBehavior()
    {
        ResumeBehaviour();
    }

    public void ResumeBehaviour()
    {

        if (_unitMovement)
            _unitMovement.ReadingMovement = true;


        walkingAround = false;

        if (!isStatic)
        {
            walkingAround = true;
            StartWalkAround();
        }
    }

    public void PauseBehavior()
    {
        SetEntityMovement(Vector3.zero);

        walkingAround = false;
        if (walkAroundRoutine != null)
        {
            StopCoroutine(walkAroundRoutine);
        }
    }

    public void SetEntityMovement(Vector3 target)
    {
        _unitMovement.MovementValue = target;
    }

    public void SetEntityLookingDirection(Vector3 dir)
    {
        _unitMovement.LookingDir = GetLookingDir(dir);
    }

    public Looking GetLookingDir(Vector3 dir)
    {
        if (dir == Vector3.right) return Looking.Right;
        else if (dir == Vector3.left) return Looking.Left;
        else if (dir == Vector3.down) return Looking.Down;
        else if (dir == Vector3.up) return Looking.Up;

        return Looking.Right;
    }

    public Looking GetLookingDir2(Vector3 lookingVector, bool opposingDir = false)
    {
        if (opposingDir) lookingVector *= -1f;

        if (Mathf.Abs(lookingVector.x) >= Mathf.Abs(lookingVector.y))
            return lookingVector.x >= 0 ? Looking.Right : Looking.Left;
        else
            return lookingVector.y >= 0 ? Looking.Up : Looking.Down;
    }

    private Looking GetDirectionFromTwoPositions(Vector3 p1, Vector3 p2)
    {
        Vector3 norm = (p2 - p1).normalized;
        //Debug.Log("p1:" + p1 + " p2:" + p2 + " norm:" + norm);
        norm = new Vector3(RoundToInt(norm.x), RoundToInt(norm.y));

        return GetLookingDir(norm);
    }

    private int RoundToInt(float f)
    {
        if(f < -0.4f)
        {
            return -1;
        }
        else if(f > 0.4f)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private Vector3 GetCurrentPos()
    {
        return walkPositions[currentWalkPosIndex];
    }

    private void ReachedToCurrentPos()
    {

        if (currentWalkPosIndex == walkPositions.Count - 1)
        {
            nextPos = -1;
        }
        else if (currentWalkPosIndex == 0)
        {
            nextPos = 1;
        }

        currentWalkPosIndex += nextPos;
    }
    private bool Raycast()
    {
        float radius = 0.35f;

        Vector3 startingPos = transform.position + (vDirection * radius);

        Debug.DrawLine(startingPos, startingPos + (vDirection * radius), Color.yellow);

        var colliders = Physics2D.OverlapCircleAll(startingPos, radius);

        foreach (var col in colliders)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                return true;
            }

        }
        return false;
    }


    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player") && trigger)
        {
            if (Raycast())
            {
                //The player is in front of me
                trigger = false;
                StopThePlayer();
            }
        }
    }

    public void StopThePlayer()
    {
        PauseBehavior();

        LookAtPlayer(_unitMovement.LookingDir);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !trigger)
        {
            trigger = true;
            ResumeBehaviour();
        }
    }

    public void ResumeAfterConversation()
    {
        isChatting = false;
        if (idleRotation)
        {
            StartIdleRotation();
        }
        if (chatStartByPlayer)
        {
            chatStartByPlayer = false;
            ResumeBehaviour();
        }
    }

    private void DisableMovement()
    {
        _unitMovement.ToggleMovement(false);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (transform.childCount > 0 && UnityEditor.Selection.activeGameObject != null)
        {
            if (UnityEditor.Selection.activeGameObject.transform.IsChildOf(transform))
            {
                Gizmos.DrawLine(transform.position, transform.GetChild(0).position);
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                }
            }
        }
#endif
    }
}