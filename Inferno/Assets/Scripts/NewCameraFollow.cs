using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Camera Follow Settings")]
    public float smoothSpeed = 0.1f;
    public Vector3 defaultOffset = new Vector3(0, 10, -10);

    [Header("Orbit Settings")]
    public float rotationSpeed = 3f;
    public float pitchSpeed = 2f;
    public float minPitch = 10f;
    public float maxPitch = 80f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoomDistance = 3f;
    public float maxZoomDistance = 20f;

    private float currentZoomDistance;
    private float targetZoomDistance;
    private float yaw = 0f;
    private float pitch = 45f;
    private bool isRotating = false;

    void Start()
    {
        currentZoomDistance = defaultOffset.magnitude;
        targetZoomDistance = currentZoomDistance;

        // Initialize yaw and pitch from default offset
        Vector3 angles = Quaternion.LookRotation(-defaultOffset).eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        HandleCameraInput();
        HandleZoom();

        // Compute desired offset from yaw, pitch, and zoom distance
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredOffset = rotation * new Vector3(0, 0, -currentZoomDistance);

        // Smooth follow
        Vector3 desiredPosition = player.position + desiredOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Always look at the player
        transform.LookAt(player);
    }

    void HandleCameraInput()
    {
        if (Input.GetMouseButton(1)) // RMB held
        {
            isRotating = true;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * pitchSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else if (isRotating) // RMB just released
        {
            // Smoothly return to default yaw/pitch
            Quaternion targetRot = Quaternion.LookRotation(-defaultOffset);
            Vector3 targetAngles = targetRot.eulerAngles;

            yaw = Mathf.LerpAngle(yaw, targetAngles.y, Time.deltaTime * rotationSpeed);
            pitch = Mathf.Lerp(pitch, targetAngles.x, Time.deltaTime * pitchSpeed);

            // Stop rotating when close enough
            if (Mathf.Abs(yaw - targetAngles.y) < 0.5f && Mathf.Abs(pitch - targetAngles.x) < 0.5f)
            {
                yaw = targetAngles.y;
                pitch = targetAngles.x;
                isRotating = false;
            }
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetZoomDistance -= scroll * zoomSpeed;
            targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
        }

        currentZoomDistance = Mathf.Lerp(currentZoomDistance, targetZoomDistance, Time.deltaTime * zoomSpeed);
    }
}
