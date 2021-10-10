using UnityEngine;

namespace NullSave.TOCK
{
    [HierarchyIcon("retro-character", false)]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class RetroCharacter : MonoBehaviour
    {

        #region Enumerations

        public enum LedgeProtection
        {
            None = 0,
            NeverDrop = 1,
            DropAfterTime = 2
        }

        public enum BasicDirection
        {
            Left = 1,
            Front = 2,
            Right = 4,
            Back = 8
        }

        #endregion

        #region Variables

        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public float minInputValue = 0.1f;
        public bool lockToGrid = true;
        public float gridSize = 0.5f;
        public bool useMaxInput = true;

        public float movementSpeed = 3;
        public bool instantRotation = true;
        public float standingRotationSpeed = 360;
        public float movingRotationSpeed = 360;

        public LedgeProtection ledgeProtection = LedgeProtection.None;
        public Vector3 protectionOffset = new Vector3(0, 0.5f, 0.1f);
        public float protectionHeight = 1;
        public LayerMask ledgeCulling = 1;
        public float timeToDrop = 1.5f;

        public bool forwardStop = true;
        public float yOffset = 0.5f;
        public float distance = 0.4f;
        public LayerMask forwardCulling = 1;
        public float xOffset = 0.25f;

        public float interactDistance = 1f;
        public float interactYOffset = 0.7f;
        public LayerMask interactCulling = 1;
        public string interactButton = "Interact";
        public InteractPrompt interactPrompt;
        public Transform promptParent;

        private Vector3 lastMove, rotateMove, interactPos;
        private bool gridLocking;
        private float targetX, targetZ;
        private float lastNonZeroX, lastNonZeroZ;
        private bool ledgeStop;
        private float ledgeElapsed;
        private bool stopForward;
        private bool canInteract;
        private InteractPrompt activePrompt;
        private Interactable interactTarget;
        private float nextInteractShow;

        #endregion

        #region Properties

        public Animator Animator { get; private set; }

        public CharacterController CharacterController { get; private set; }

        public Vector3 Movement { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        public Transform ViewCamera { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ViewCamera = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            UpdateInput();
            UpdateMovementGrid();
            UpdateMovement();
            UpdateLedgeProtection();
            UpdateForwardProtection();

#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif

            UpdateInteract();
        }

        private void OnDrawGizmos()
        {
            if (ledgeProtection != LedgeProtection.None)
            {
                DrawLedgeGizmo();
            }

            if (forwardStop)
            {
                DrawForwardGizmo();
            }

            DrawInteractGizmo();
        }

        private void Update()
        {

#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif

            if (nextInteractShow > 0)
            {
                nextInteractShow -= Time.deltaTime;
            }

            if (canInteract && GetButtonDown(interactButton))
            {
                InteractHide();
                interactTarget.Interact(interactPos);
                nextInteractShow = 0.1f;
            }
        }

        #endregion

        #region Private Methods

        private void DrawForwardGizmo()
        {
            Gizmos.color = stopForward ? Color.red : Color.blue;

            Vector3 start = transform.position + transform.up * yOffset;
            Vector3 end = start + transform.forward * distance;
            Gizmos.DrawLine(start, end);

            start += transform.right * xOffset / 2;
            end += transform.right * xOffset / 2;
            Gizmos.DrawLine(start, end);

            start -= transform.right * xOffset;
            end -= transform.right * xOffset;
            Gizmos.DrawLine(start, end);

        }

        private void DrawInteractGizmo()
        {
            Gizmos.color = canInteract ? Color.white : Color.yellow;

            Vector3 start = transform.position + transform.up * interactYOffset;
            Vector3 end = start + transform.forward * interactDistance;
            Gizmos.DrawLine(start, end);

            if (canInteract)
            {
                Gizmos.DrawSphere(interactPos, 0.1f);
            }
        }

        private void DrawLedgeGizmo()
        {
            Gizmos.color = ledgeStop ? Color.red : Color.blue;
            Vector3 start = transform.position + transform.forward * protectionOffset.z + transform.up * protectionOffset.y + transform.right * protectionOffset.x;
            Vector3 end = start - transform.up * protectionHeight;
            Gizmos.DrawLine(start, end);
        }

        private float GetAxis(string axisName)
        {
#if GAME_COG
            if (GameCog.Input != null)
            {
                return GameCog.Input.GetAxis(axisName);
            }
#endif

            return Input.GetAxis(axisName);
        }

        private bool GetButtonDown(string buttonName)
        {
#if GAME_COG
            if (GameCog.Input != null)
            {
                return GameCog.Input.GetButtonDown(buttonName);
            }
#endif
            return Input.GetButtonDown(buttonName);
        }

        private void InteractHide()
        {
            if (activePrompt == null) return;
            Destroy(activePrompt.gameObject);
            activePrompt = null;
        }

        private void InteractShow()
        {
            if (nextInteractShow <= 0)
            {
                activePrompt = Instantiate(interactPrompt, promptParent);
                activePrompt.SetText(interactTarget.displayText);
            }
        }

        private void UpdateForwardProtection()
        {
            RaycastHit hit;
            Vector3 start = transform.position + transform.up * yOffset;
            if (Physics.Raycast(start, transform.forward, out hit, distance, forwardCulling))
            {
                stopForward = true;
                return;
            }

            start += transform.right * xOffset / 2;
            if (Physics.Raycast(start, transform.forward, out hit, distance, forwardCulling))
            {
                stopForward = true;
                return;
            }

            start -= transform.right * xOffset;
            if (Physics.Raycast(start, transform.forward, out hit, distance, forwardCulling))
            {
                stopForward = true;
                return;
            }

            stopForward = false;
        }

        private void UpdateInput()
        {
            float v = GetAxis(inputVertical);
            float h = GetAxis(inputHorizontal);

#if GAME_COG
            if (GameCog.IsModalVisible)
            {
                v = h = 0;
            }
#endif

            // Adjust inputs
            if (useMaxInput)
            {
                if (v <= -minInputValue)
                {
                    v = -1;
                }
                else if (v >= minInputValue)
                {
                    v = 1;
                }
                else v = 0;

                if (h <= -minInputValue)
                {
                    h = -1;
                }
                else if (h >= minInputValue)
                {
                    h = 1;
                }
                else h = 0;
            }
            else
            {
                if (v > -minInputValue && v < minInputValue) v = 0;
                if (h > -minInputValue && h < minInputValue) h = 0;
            }

            // Normalize input
            if (ViewCamera != null)
            {
                Movement = v * Vector3.Scale(ViewCamera.forward, new Vector3(1, 0, 1)).normalized + h * ViewCamera.right;
            }
            else
            {
                Movement = v * Vector3.forward + h * Vector3.right;
            }
        }

        private void UpdateInteract()
        {
            Interactable obj;
            RaycastHit hit;
            Vector3 start = transform.position + transform.up * interactYOffset;
            if (Physics.Raycast(start, transform.forward, out hit, interactDistance, interactCulling))
            {
                obj = hit.transform.gameObject.GetComponentInChildren<Interactable>();
                if (obj != null)
                {
                    interactTarget = obj;
                    interactPos = hit.point;

                    if (!canInteract || activePrompt == null)
                    {
                        canInteract = true;
                        InteractShow();
                    }

                    return;
                }
            }

            if (canInteract)
            {
                canInteract = false;
                InteractHide();
            }
        }

        private void UpdateLedgeProtection()
        {
            if (ledgeProtection == LedgeProtection.None) return;
            if (Movement.magnitude == 0)
            {
                ledgeElapsed = 0;
                //ledgeStop = false;
                //return;
            }

            Vector3 start = transform.position + transform.forward * protectionOffset.z + transform.up * protectionOffset.y + transform.right * protectionOffset.x;
            if (Physics.Raycast(start, -transform.up, protectionHeight, ledgeCulling))
            {
                ledgeElapsed = 0;
                ledgeStop = false;
            }
            else
            {
                if (ledgeProtection == LedgeProtection.NeverDrop)
                {
                    ledgeStop = true;
                }
                else
                {
                    ledgeElapsed += Time.deltaTime;
                    ledgeStop = ledgeElapsed < timeToDrop;
                }
            }
        }

        private void UpdateMovement()
        {
            Vector3 move = Movement;
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);

            RaycastHit hitInfo;
            Vector3 groundNormal = Vector3.up;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, 0.3f))
            {
                groundNormal = hitInfo.normal;
            }

            move = Vector3.ProjectOnPlane(move, groundNormal);
            float turnAmount = Mathf.Atan2(move.x, move.z);

            // Apply rotation
            if (!instantRotation)
            {
                float turnSpeed = Mathf.Lerp(standingRotationSpeed, movingRotationSpeed, move.z);
                transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
            }
            else if (Movement.magnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(Movement);
            }

            // Set minimum distance
            if (Movement.magnitude > 0)
            {
                lastMove = Movement;
                if (lastMove.x != 0) lastNonZeroX = lastMove.x;
                if (lastMove.z != 0) lastNonZeroZ = lastMove.z;
            }

            // Update animator
            Animator.SetFloat("Forward", move.z, 0.1f, Time.deltaTime);

            bool interrupt = stopForward;
#if GAME_COG
            interrupt |= GameCog.IsModalVisible;
#endif

            if (interrupt)
            {
                CharacterController.SimpleMove(Vector3.zero);
                lastMove = Vector3.zero;
                return;
            }

            if (!ledgeStop)
            {
                CharacterController.SimpleMove(Movement * movementSpeed * Time.deltaTime);

                if (gridLocking)
                {
                    if (Movement.x > 0)
                    {
                        if (transform.position.x > targetX) transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
                    }
                    else if (Movement.x < 0)
                    {
                        if (transform.position.x < targetX) transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
                    }

                    if (transform.position.x % gridSize == 0)
                    {
                        lastMove.x = 0;
                    }

                    if (Movement.z > 0)
                    {
                        if (transform.position.z > targetZ) transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
                    }
                    else if (Movement.z < 0)
                    {
                        if (transform.position.z < targetZ) transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
                    }

                    if (transform.position.z % gridSize == 0)
                    {
                        lastMove.z = 0;
                    }

                    if (lastMove.x == 0 && lastMove.z == 0)
                    {
                        transform.rotation = Quaternion.LookRotation(rotateMove);
                        gridLocking = false;
                    }
                }
            }
            else if (ledgeProtection != LedgeProtection.None)
            {
                CharacterController.SimpleMove(Vector3.zero);
                lastMove = Vector3.zero;
            }
        }

        private void UpdateMovementGrid()
        {
            if (!lockToGrid || Movement.magnitude != 0) return;

            if (!gridLocking)
            {
                if (transform.position.x % gridSize != 0)
                {
                    if (lastMove.x == 0) lastMove = new Vector3(lastNonZeroX, 0, lastMove.z);
                    gridLocking = true;
                    Movement = lastMove;
                    if (Movement.x > 0)
                    {
                        targetX = Mathf.Ceil(transform.position.x / gridSize) * gridSize;
                    }
                    else if (Movement.x < 0)
                    {
                        targetX = (Mathf.Ceil(transform.position.x / gridSize) - 1) * gridSize;
                    }
                }

                if (transform.position.z % gridSize != 0)
                {
                    if (lastMove.z == 0) lastMove = new Vector3(lastMove.x, 0, lastNonZeroZ);
                    gridLocking = true;
                    Movement = lastMove;
                    if (Movement.z > 0)
                    {
                        targetZ = Mathf.Ceil(transform.position.z / gridSize) * gridSize;
                    }
                    else if (Movement.z < 0)
                    {
                        targetZ = (Mathf.Ceil(transform.position.z / gridSize) - 1) * gridSize;
                    }
                }

                rotateMove = lastMove;
            }
            else
            {
                Movement = lastMove;
            }
        }

        #endregion

    }
}