using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    Forward,
    Backward,
    Right,
    Left
}

public class CameraController : MonoBehaviour
{
    public Rigidbody target;
    [SerializeField, Range(0, 1)] private float smoothMoveSpeed = 0.125f;
    [SerializeField, Range(0, 5)] private float smoothZoomSpeed = 0.125f;
    [SerializeField, Range(0, 10)] private float smoothZoomPower = 0.125f;
    [SerializeField, Range(0, 100)] private float sizeDelta;
    [SerializeField, Range(10, 100)] private float size;
    public Transform[] cameraPositions;
    private Transform targetPosition;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        targetPosition = cameraPositions[0];
    }

    private void FixedUpdate()
    {
        if (!target) return;
        transform.position = Vector3.Lerp(transform.position, targetPosition.position, smoothMoveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetPosition.rotation, smoothMoveSpeed);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, Mathf.Clamp(size + target.velocity.magnitude * smoothZoomPower, size, size + sizeDelta), smoothZoomSpeed);
    }

    public void ChangeCameraAngle(float velocityX)
    {
        byte index = 0;
        if(velocityX > 2.5f) index = 1;
        else if(velocityX < -2.5f) index = 2;
        targetPosition = cameraPositions[index];
    }
}