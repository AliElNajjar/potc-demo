using UnityEngine;

namespace NullSave.TOCK.Character
{
    [CreateAssetMenu(menuName = "TOCK/Character Plugins/Unity Character Controller Movement")]
    public class UnityCCMovement : CharacterCogPlugin
    {

        #region Variables

        public float movementSpeed = 3;
        public bool instantRotation = true;
        public float standingRotationSpeed = 360;
        public float movingRotationSpeed = 360;
        public string movementAnimName = "MoveSpeed";

        #endregion

        #region Plugin Methods

        public override void Movement()
        {
            if (!Character.CharacterController.enabled || Character.Movement.magnitude < 0.1f)
            {
                Character.Animator.SetFloat(movementAnimName, 0, 0.1f, Time.deltaTime);
                return;
            }

            Vector3 move = Character.Movement;
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);

            Vector3 groundNormal = Vector3.up;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, 0.3f))
            {
                groundNormal = hitInfo.normal;
            }

            // Apply rotation
            if (!Character.InAction && Character.Movement.magnitude > 0)
            {
                if (!instantRotation)
                {
                    float turnAmount = Mathf.Atan2(move.x, move.z);
                    float turnSpeed = Mathf.Lerp(standingRotationSpeed, movingRotationSpeed, move.z);
                    transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(Character.Movement);
                }
            }

            if (Character.PreventMovement || Character.InAction)
            {
                Character.Animator.SetFloat(movementAnimName, 0, 0.1f, Time.deltaTime);
                return;
            }

            move = Vector3.ProjectOnPlane(move, groundNormal);

            // Update Animator
            Character.Animator.SetFloat(movementAnimName, move.z, 0.1f, Time.deltaTime);
        }

        #endregion

    }
}