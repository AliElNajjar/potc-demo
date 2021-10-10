using UnityEngine;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class BasicPersonController : MonoBehaviour
    {

        #region Variables

        public float m_MovingTurnSpeed = 360;
        public float m_StationaryTurnSpeed = 180;
        public float m_JumpPower = 12f;
        [Range(1f, 4f)] public float m_GravityMultiplier = 2f;
        public float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        public float m_MoveSpeedMultiplier = 1f;
        public float m_AnimSpeedMultiplier = 1f;
        public float m_GroundCheckDistance = 0.1f;


        public string horizontalMove = "Horizontal";
        public string verticalMove = "Vertical";
        public bool lockInput = false;
        public bool lockCursor = true;

        public GameObject gameOver;

        public string forwardAnimName = "Forward";
        public bool setTurnAnim = true;
        public string turnAnimName = "Turn";
        public bool setGroundedAnim = true;
        public string GroundedAnimName = "OnGround";
        public bool canCrouch = true;
        public string crouchAnimName = "Crouch";
        public bool canJump = false;
        public string jumpAnimName = "Jump";

        Rigidbody m_Rigidbody;
        Animator m_Animator;
        bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        const float k_Half = 0.5f;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;
        float m_CapsuleHeight;
        Vector3 m_CapsuleCenter;
        CapsuleCollider m_Capsule;
        bool m_Crouching;

        private BasicPersonController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        //private bool m_Crouch;
        private bool m_Sprint;

#if STATS_COG

        private Stats.StatsCog statsCog;
        public string healthStatName = "Health";
        private Stats.StatValue health;

#endif

#if INVENTORY_COG
        private Inventory.InventoryCog inventory;
#endif

        #endregion

        #region Properties

        public bool LockInput
        {
            get { return lockInput; }
            set { lockInput = value; }
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;

            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<BasicPersonController>();

#if STATS_COG
            statsCog = GetComponentInChildren<Stats.StatsCog>();
            if(statsCog != null) health = statsCog.FindStat(healthStatName);
#endif

#if INVENTORY_COG
            inventory = GetComponent<Inventory.InventoryCog>();
#endif
        }

        private void Update()
        {
            if (InputLocked()) return;

            if (canJump && !m_Jump)
            {
                m_Jump = Input.GetKeyDown(KeyCode.Space);
            }

            UpdateHealth();
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (InputLocked()) return;

            // read inputs
            float h = Input.GetAxis(horizontalMove);
            float v = Input.GetAxis(verticalMove);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

            // pass all parameters to the character control script
            //m_Character.Move(m_Move, m_Crouch, m_Jump);
            Move(m_Move, false, m_Jump);
            m_Jump = false;
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
            }
        }

        #endregion

        #region Public Methods

        public void Move(Vector3 move, bool crouch, bool jump)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (m_IsGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }

        #endregion

        #region Private Methods

        private bool InputLocked()
        {
            if (lockInput) return true;
#if GAME_COG
            if (GameCog.IsModalVisible) return true;
#endif
#if INVENTORY_COG
            if (inventory != null && inventory.IsMenuOpen) return true;
#endif
            return false;
        }

        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch && canCrouch)
            {
                if (m_Crouching) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Crouching = false;
            }
        }

        void PreventStandingInLowHeadroom()
        {
            // prevent standing up in crouch-only zones
            if (!m_Crouching && canCrouch)
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                }
            }
        }

        void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            m_Animator.SetFloat(forwardAnimName, m_ForwardAmount, 0.1f, Time.deltaTime);
            if(setTurnAnim) m_Animator.SetFloat(turnAnimName, m_TurnAmount, 0.1f, Time.deltaTime);
            if(canCrouch) m_Animator.SetBool(crouchAnimName, m_Crouching);
            if (setGroundedAnim) m_Animator.SetBool(GroundedAnimName, m_IsGrounded);

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_IsGrounded && move.magnitude > 0)
            {
                m_Animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                m_Animator.speed = 1;
            }
        }

        void HandleAirborneMovement()
        {
            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }

        void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump)
            {
                if (!crouch && m_Animator.GetBool(GroundedAnimName))
                {
                    // jump!
                    m_IsGrounded = false;
                    m_GroundCheckDistance = 0.15f;
                    m_Animator.SetTrigger(jumpAnimName);
                }
            }
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
            }
        }

        private void UpdateHealth()
        {
#if STATS_COG
            if (health != null && health.CurrentValue <= 0 && !gameOver.activeSelf)
            {
                gameOver.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
#endif
        }

        #endregion

    }
}