using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave
{
    [RequireComponent(typeof(Image))]
    public class ImageAnimation : MonoBehaviour
    {

        #region Variables

        public List<Sprite> animationSprite;
        public float secPerFrame = 0.1f;
        public bool loop = true;

        private float elapsed;
        private Image image;
        private int index;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            image = GetComponent<Image>();
            index = 0;
            image.sprite = animationSprite[index];
            elapsed = 0;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= secPerFrame)
            {
                index += 1;
                if (index == animationSprite.Count - 1)
                {
                    if (loop)
                    {
                        index = 0;
                    }
                    else
                    {
                        enabled = false;
                        return;
                    }
                }
                image.sprite = animationSprite[index];
                elapsed = elapsed - secPerFrame ;
            }
        }

        #endregion

    }
}