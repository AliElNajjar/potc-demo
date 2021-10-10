using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(StatsCog))]
    public class Knockback : MonoBehaviour
    {

        #region Variables

        public float knockbackDistance = 1.5f;
        public float knockbackDuration = 0.5f;

        private float elapsed = 0;
        private Vector3 start, dest;

        #endregion

        #region Properties

        public CharacterController Controller { get; private set; }

        public StatsCog Stats { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Stats = GetComponentInChildren<StatsCog>();
            Stats.onHitDirection.AddListener(StartKnockback);
            Controller = GetComponentInChildren<CharacterController>();
        }

        #endregion

        #region Private Methods

        private void StartKnockback(HitDirection direction)
        {
            elapsed = 0;
            start = transform.position;

            switch (direction)
            {
                case HitDirection.BackCenter:
                    dest = transform.position + (transform.forward * knockbackDistance);
                    break;
                case HitDirection.BackLeft:
                    dest = transform.position + ((transform.forward * knockbackDistance) + (transform.right * knockbackDistance)) / 2;
                    break;
                case HitDirection.BackRight:
                    dest = transform.position + ((transform.forward * knockbackDistance) - (transform.right * knockbackDistance)) / 2;
                    break;
                case HitDirection.FrontRight:
                    dest = transform.position - ((transform.forward * knockbackDistance) + (transform.right * knockbackDistance)) / 2;
                    break;
                case HitDirection.FrontLeft:
                    dest = transform.position - ((transform.forward * knockbackDistance) - (transform.right * knockbackDistance)) / 2;
                    break;
                case HitDirection.FrontCenter:
                    dest = transform.position - (transform.forward * knockbackDistance);
                    break;
                case HitDirection.Left:
                    dest = transform.position + (transform.right * knockbackDistance);
                    break;
                case HitDirection.Right:
                    dest = transform.position - (transform.right * knockbackDistance);
                    break;
            }

            StartCoroutine("DoKnockback");
        }

        private IEnumerator DoKnockback()
        {
            if (Controller) Controller.enabled = false;
            while (elapsed < knockbackDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, dest, elapsed / knockbackDuration);
                yield return new WaitForEndOfFrame();
            }
            if (Controller) Controller.enabled = true;
        }

        #endregion

    }
}