using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructorUI : MonoBehaviour
{
    public bool destroyedOnClick;
    [Range(1, 5)] public float timeToDestroy;

    private float countDown;

    private void OnEnable()
    {
        countDown = timeToDestroy;
    }

    private void Update()
    {
        if (destroyedOnClick)
        {
            if (Input.anyKeyDown)
            {
                gameObject.SetActive(false);
            }
        }
        
        if (countDown > 0f)
        {
            countDown -= Time.unscaledDeltaTime;
            return;
        }

        if(countDown <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
