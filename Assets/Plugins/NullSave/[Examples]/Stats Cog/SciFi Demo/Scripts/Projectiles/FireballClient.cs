using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class FireballClient : MonoBehaviour
    {

        #region Variables

        public StatsCog statsCog;
        public Fireball prefab;
        public Animator animator;
        public Transform reticle;

        private Fireball fireball;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (fireball != null)
            {
                AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(0);
                if (myAnimatorClip[0].clip.name == "Throw")
                {
                    float myTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
                    if (myTime >= 0.9f)
                    {
                        Ray ray = RectTransformUtility.ScreenPointToRay(Camera.main, reticle.position);
                        Vector3 lookAt = ray.origin + Camera.main.transform.forward * 3;

                        fireball.gameObject.SetActive(false);
                        fireball.transform.position = fireball.transform.position + (fireball.transform.right * 1.0f);
                        fireball.transform.SetParent(null);
                        fireball.transform.LookAt(ray.GetPoint(50));
                        Debug.DrawLine(ray.origin, ray.GetPoint(50));

                        DamageDealer dealer = fireball.GetComponentInChildren<DamageDealer>();
                        if (dealer != null)
                        {
                            dealer.overrideDamageSource = true;
                            dealer.damageSource = statsCog.gameObject;
                        }

                        fireball.IsReleased = true;
                        fireball.gameObject.SetActive(true);
                        fireball = null;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void LaunchFireball()
        {
            Debug.Log("Launch fireball");
            fireball = Instantiate(prefab, transform);
            fireball.GetComponent<DamageDealer>().StatsSource = statsCog;

            Vector3 orgRot = animator.gameObject.transform.rotation.eulerAngles;
            animator.gameObject.transform.rotation = Quaternion.Euler(orgRot.x, Camera.main.transform.rotation.eulerAngles.y, orgRot.z);

        }

        #endregion

    }
}