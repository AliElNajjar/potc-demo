using UnityEngine;

namespace NullSave.TOCK
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioDieOnComplete : MonoBehaviour
    {

        #region Variables

        private new AudioSource audio;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (audio.time >= audio.clip.length || !audio.isPlaying)
            {
                Destroy(gameObject);
            }
        }

        #endregion

    }
}