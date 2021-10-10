using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK
{
    [HierarchyIcon("camera")]
    public class BasicShoulderCamera : MonoBehaviour
    {

        #region Structures

        private struct ClipPlanePoints
        {
            public Vector3 UpperLeft;
            public Vector3 UpperRight;
            public Vector3 LowerLeft;
            public Vector3 LowerRight;
        }

        #endregion

        #region Variables

        // Camera State
        public float defaultDistance = 1.5f;
        public float maxDistance = 3f;
        public float minDistance = 0.5f;
        public float height = 0.8f;
        public float smoothFollow = 10f;
        public float smoothRotation = 10f;
        public float xMouseSensitivity = 3f;
        public float yMouseSensitivity = 3f;
        public float xOffset = 1;

        public float cullingHeight = 0.2f;
        public float cullingMinDist = 0.1f;
        public float stateSmoothing = 6f;

        // Camera
        public Transform target;

        // Input
        public string inputHoriz = "Mouse X";
        public string inputVert = "Mouse Y";
        public LayerMask cullingLayer = 1 << 0;
        public float clipPlaneMargin;
        public float checkHeightRadius;

        // Exposed for now
        private float distance;
        private Transform lookAt;
        private float mouseX, mouseY;
        private Vector2 movementSpeed;
        private float currentZoom;
        private float cullingDistance;
        private float currentHeight;
        private Vector3 desired_cPos, current_cPos;
        private Vector3 lookTargetAdjust;
        private new Camera camera;
        private bool lastWasBlocked;

#if INVENTORY_COG
        private Inventory.InventoryCog inventory;
#endif

        #endregion

        #region Properties

        public bool LockInput { get; set; }

        #endregion

        #region Unity Methods

        public void Start()
        {
            FindPlayer();

            camera = GetComponent<Camera>();

            distance = Vector3.Distance(transform.position, target.position);

            // Setup Look At
            if (!lookAt) lookAt = new GameObject(name + ".LookAt").transform;
            lookAt.position = target.position;
            lookAt.rotation = transform.rotation;
            lookAt.hideFlags = HideFlags.HideInHierarchy;
        }

        public void Update()
        {
            UpdateInput();
            UpdateMovement();
        }

        #endregion

        #region Private Methods

        private bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask cullingLayer, Color color)
        {
            bool value = false;

            if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                cullingDistance = hitInfo.distance;
            }

            if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
            }

            if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
            }

            if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
            }

            return value;
        }

        private void FindPlayer()
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
            {
                target = go.gameObject.transform;
#if INVENTORY_COG
                inventory = target.gameObject.GetComponent<Inventory.InventoryCog>();
#endif
            }
        }

        private bool InputLocked()
        {
            if (LockInput) return true;
#if INVENTORY_COG
            if (inventory != null && inventory.IsMenuOpen) return true;
#endif
            return false;
        }

        private ClipPlanePoints NearClipPlanePoints(Vector3 pos, float clipPlaneMargin)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;
            height *= 1 + clipPlaneMargin;
            width *= 1 + clipPlaneMargin;
            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }

        private void UpdateInput()
        {
            float x = 0;
            float y = 0;

            if (!InputLocked())
            {
                x = Input.GetAxis(inputHoriz);
                y = Input.GetAxis(inputVert);
            }
            mouseY -= y * yMouseSensitivity;
            movementSpeed.y = -y;

            mouseX += x * xMouseSensitivity;
            movementSpeed.x = x;

            mouseY = Mathf.Clamp(mouseY, -40, 70);
        }

        private void UpdateMovement()
        {
            if (!lastWasBlocked) distance = Mathf.Lerp(distance, defaultDistance, smoothFollow * Time.deltaTime);
            currentZoom = defaultDistance;

            cullingDistance = Mathf.Lerp(cullingDistance, currentZoom, stateSmoothing * Time.deltaTime);
            var camDir = (-1 * lookAt.forward);

            camDir = camDir.normalized;

            var targetPos = new Vector3(target.position.x, target.position.y, target.position.z) + target.transform.up;
            desired_cPos = targetPos + target.transform.up * height;
            current_cPos = targetPos + target.transform.up * currentHeight;

            ClipPlanePoints planePoints = NearClipPlanePoints(current_cPos + (camDir * (distance)), clipPlaneMargin);
            ClipPlanePoints oldPoints = NearClipPlanePoints(desired_cPos + (camDir * currentZoom), clipPlaneMargin);

            //Check if Height is not blocked 
            if (Physics.SphereCast(targetPos, checkHeightRadius, target.transform.up, out RaycastHit hitInfo, cullingHeight + 0.2f, cullingLayer))
            {
                var t = hitInfo.distance - 0.2f;
                t -= height;
                t /= (cullingHeight - height);
                cullingHeight = Mathf.Lerp(height, cullingHeight, Mathf.Clamp(t, 0.0f, 1.0f));
            }
            else
            {
                cullingHeight = Mathf.Lerp(cullingHeight, cullingHeight, stateSmoothing * Time.deltaTime);
            }

            //Check if desired target position is not blocked       
            if (CullingRayCast(desired_cPos, oldPoints, out hitInfo, currentZoom + 0.2f, cullingLayer, Color.blue))
            {
                lastWasBlocked = true;

                if (distance < defaultDistance)
                {
                    var t = hitInfo.distance;
                    t -= cullingMinDist;
                    t /= (currentZoom - cullingMinDist);
                    currentHeight = Mathf.Lerp(cullingHeight, height, Mathf.Clamp(t, 0.0f, 1.0f));
                    current_cPos = targetPos + target.transform.up * currentHeight;
                }
            }
            else
            {
                currentHeight = Mathf.Lerp(currentHeight, height, stateSmoothing * Time.deltaTime);
                lastWasBlocked = false;
            }

            //Check if target position with culling height applied is not blocked
            if (CullingRayCast(current_cPos, planePoints, out hitInfo, distance, cullingLayer, Color.cyan)) distance = Mathf.Clamp(cullingDistance, 0.0f, defaultDistance);
            var lookPoint = current_cPos + lookAt.forward * 2f;
            lookPoint += (lookAt.right * Vector3.Dot(camDir * (distance), lookAt.right));
            lookAt.position = current_cPos;

            Quaternion newRot = Quaternion.Euler(mouseY, mouseX, 0);
            lookAt.rotation = Quaternion.Lerp(lookAt.rotation, newRot, smoothRotation * Time.deltaTime);
            transform.position = current_cPos + (camDir * (distance)) + transform.right * xOffset;
            var rotation = Quaternion.LookRotation((lookPoint) - transform.position);

            lookTargetAdjust.x = Mathf.LerpAngle(lookTargetAdjust.x, 0, smoothFollow * Time.deltaTime);
            lookTargetAdjust.y = Mathf.LerpAngle(lookTargetAdjust.y, 0, smoothFollow * Time.deltaTime);
            lookTargetAdjust.z = Mathf.LerpAngle(lookTargetAdjust.z, 0, smoothFollow * Time.deltaTime);

            Vector3 euler = rotation.eulerAngles + lookTargetAdjust;
            transform.rotation = Quaternion.Euler(new Vector3(euler.x, euler.y, 0));
            movementSpeed = Vector2.zero;

            target.rotation = Quaternion.Lerp(target.rotation, Quaternion.Euler(0, mouseX, 0), smoothRotation * Time.deltaTime);

        }

        #endregion

    }
}