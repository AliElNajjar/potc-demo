using NullSave.TOCK.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class SlimeBehaviour : MonoBehaviour
    {

        #region Enumerations

        public enum MovementState
        {
            Idle,
            InPursuit,
            Returning,
            MeleeAttack
        }

        #endregion

        #region Variables

        public TargetDetection targetDetection;
        public float timeToStopPursuit = 3f;

        public bool canMelee = true;
        public float maxMeleeDist = 1f;
        public DamageDealer meleeDamageDealer;
        public float meleeAttackRest = 2.5f;

        public bool canRanged = true;
        public float maxRangedDist = 8f;

        public GameObject deathParticle;

        public string healthStat = "Health";
        private StatValue health;

        private Transform _target;
        private MovementState movementState;
        private float m_TimerSinceLostTarget;
        private Vector3 origin;
        private Quaternion orgRot;
        private bool inAttack;
        private float attackCheck;
        private float nextMeleeAttack;
        private bool isDead;
        private float destroyRemain;

        private Vector3 startPosition;
        private Quaternion startRot;
        public bool autoRespawn = false;
        public GameObject respawnObject;
        public float respawnTime = 10;

        #endregion

        #region Properties

        public Animator Animator { get; private set; }

        public NavMeshAgent NavMeshAgent { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            origin = transform.position;
            orgRot = transform.rotation;
        }

        private void Start()
        {
            health = GetComponent<StatsCog>().FindStat(healthStat);
            startPosition = transform.position;
            startRot = transform.rotation;
        }

        private void FixedUpdate()
        {
            if (isDead)
            {
                Animator.SetFloat("Forward", 0);
                return;
            }

            FindTarget();
            UpdateAttack();

            if (inAttack) return;

            switch (movementState)
            {
                case MovementState.Idle:
                    Animator.SetFloat("Forward", 0);
                    break;
                case MovementState.InPursuit:
                    Pursue();
                    Animator.SetFloat("Forward", 1);
                    break;
                case MovementState.Returning:
                    Animator.SetFloat("Forward", 1);
                    if (Vector3.Distance(transform.position, origin) <= 0.2f)
                    {
                        Animator.SetFloat("Forward", 0);
                        transform.position = origin;
                        transform.rotation = orgRot;
                    }
                    break;
            }
        }

        private void Update()
        {
            if (!isDead)
            {
                UpdateHealth();
            }
            else
            {
                destroyRemain -= Time.deltaTime;
                if (destroyRemain <= 0)
                {
                    if (autoRespawn)
                    {
                        GameObject goRespawn = new GameObject(name + " respawner");
                        goRespawn.transform.position = startPosition;
                        goRespawn.transform.rotation = startRot;
                        EnemySpawner spawner = goRespawn.AddComponent<EnemySpawner>();
                        spawner.objectToSpawn = respawnObject;
                        spawner.spawnTime = respawnTime;
                        spawner.destroyAfterSpawn = true;
                    }
                    Destroy(gameObject);
                    if (deathParticle != null)
                    {
                        GameObject go = Instantiate(deathParticle);
                        go.transform.position = transform.position;
                        go.transform.rotation = transform.rotation;
                    }
                }
            }
        }

        private void OnAnimatorMove()
        {
            if (isDead || Time.deltaTime == 0)
            {
                NavMeshAgent.velocity = Vector3.zero;
            }
            else
            {
                NavMeshAgent.velocity = (Animator.deltaPosition / Time.deltaTime) * NavMeshAgent.speed;
            }
        }

        #endregion

        #region Private Methods

        private void FindTarget()
        {
            Transform target = targetDetection.Detect(transform, _target == null);

            if (_target == null)
            {
                if (target != null)
                {
                    _target = target;
                    movementState = MovementState.InPursuit;
                }
            }
            else
            {
                if (target == null)
                {
                    m_TimerSinceLostTarget += Time.deltaTime;

                    if (m_TimerSinceLostTarget >= timeToStopPursuit)
                    {
                        Vector3 toTarget = _target.transform.position - transform.position;

                        if (toTarget.sqrMagnitude > targetDetection.detectionRadius * targetDetection.detectionRadius)
                        {
                            _target = null;
                            NavMeshAgent.SetDestination(origin);
                            movementState = MovementState.Returning;
                            Debug.Log("Returning");
                        }
                    }
                }
                else
                {
                    if (target != _target)
                    {
                        _target = target;
                        movementState = MovementState.InPursuit;
                    }

                    m_TimerSinceLostTarget = 0.0f;
                }
            }

        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            targetDetection.EditorGizmo(transform);
        }
#endif

        private void Pursue()
        {
            if (_target == null)
            {
                movementState = MovementState.Idle;
                return;
            }

            NavMeshAgent.SetDestination(_target.position);
        }

        private void UpdateAttack()
        {
            if (inAttack)
            {
                if (attackCheck > 0)
                {
                    attackCheck -= Time.deltaTime;
                }
                else
                {
                    string clipName = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    if (clipName != "Slime|Attack 1" && clipName != "Slime|Attack 2")
                    {
                        movementState = MovementState.InPursuit;
                        meleeDamageDealer.SetColliderEnabled(false);
                        inAttack = false;
                        nextMeleeAttack = meleeAttackRest;
                    }
                }

                return;
            }

            if (nextMeleeAttack > 0)
            {
                nextMeleeAttack -= Time.deltaTime;
            }

            if (_target != null)
            {
                float distance = Vector3.Distance(transform.position, _target.position);
                if (canMelee && nextMeleeAttack <= 0 && distance <= maxMeleeDist)
                {
                    movementState = MovementState.MeleeAttack;
                    Animator.SetFloat("Forward", 0);
                    Animator.SetInteger("AttackId", 0);
                    Animator.SetTrigger("Attack");
                    attackCheck = 0.3f;
                    inAttack = true;
                    meleeDamageDealer.SetColliderEnabled(true);
                }
            }
        }

        private void UpdateHealth()
        {
            if (health.CurrentValue <= 0)
            {
                isDead = true;
                destroyRemain = 1.9f;
            }

            Animator.SetBool("IsDead", isDead);
        }

        #endregion

    }
}