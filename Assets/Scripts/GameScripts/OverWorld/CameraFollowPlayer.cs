using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    private Bounds areaCameraBounds;

    void Start()
    {
        Init();
    }

    void Init()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (target == null)
            return;

        if (GameObject.Find("CameraBounds"))
            areaCameraBounds = GameObject.Find("CameraBounds").GetComponent<BoxCollider>().bounds;

        offset = new Vector3(offset.x, offset.y, transform.position.z);

        camera = Camera.main;
        goalZ = camera.transform.position.z;

        transform.position = target.transform.position + Vector3.right;
        transform.position += Vector3.forward * goalZ;
    }

    private new Camera camera;
    private readonly float lerpSpeed = 2.5f;

    private float goalZ; //the target z is consistently the same

    public void LerpCameraToPosition(Vector3 targetPosition)
    {
        targetPosition.z = goalZ;

        if (!MathUtils.Approximately(camera.transform.localPosition, targetPosition, 0.1f))
        {
            camera.transform.localPosition = Vector3.Slerp(camera.transform.localPosition, targetPosition, lerpSpeed * Time.deltaTime);
        }
        ClampCameraPosition();
    }

    private Vector3 GetTargetPositionOnEntity(Transform entity)
    {
        return GetTargetPositionRelativeToPoint(entity.localPosition);
    }

    private Vector3 GetTargetPositionRelativeToPoint(Vector3 point)
    {
        Vector3 goalPosition = new Vector3(point.x,
            point.y,
            camera.transform.localPosition.z);

        return goalPosition;
    }

    // Update is called once per frame
    public void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Battle")
        {
            this.enabled = false;
        }

        if (!target)
        {
            return;
        }

        if (!target)
        {
            return;
        }

        var goalPosition = GetTargetPositionOnEntity(target);
        //Debug.Log(goalPosition.ToString());

        //If within bounds, just go to the character's position
        LerpCameraToPosition(goalPosition);
    }

    private void ClampCameraPosition()
    {
        if (areaCameraBounds == null)
            return;

        this.transform.position = new Vector3(
            Mathf.Clamp(this.transform.position.x, areaCameraBounds.min.x, areaCameraBounds.max.x),
            Mathf.Clamp(this.transform.position.y, areaCameraBounds.min.y, areaCameraBounds.max.y),
            transform.position.z
            );
    }
}
