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

    private float yaw;
    private float pitch;
    private float tempYaw;
    private float tempPitch;

    private bool isMiddleRotating = false;

    void Start()
    {
        currentZoomDistance = defaultOffset.magnitude;
        targetZoomDistance = currentZoomDistance;

        Vector3 angles = Quaternion.LookRotation(-defaultOffset).eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        HandleCameraInput();
        HandleZoom();

        float finalYaw = isMiddleRotating ? tempYaw : yaw;
        float finalPitch = isMiddleRotating ? tempPitch : pitch;

        Quaternion rotation = Quaternion.Euler(finalPitch, finalYaw, 0);
        Vector3 desiredOffset = rotation * new Vector3(0, 0, -currentZoomDistance);
        Vector3 desiredPosition = player.position + desiredOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(player);
    }

    void HandleCameraInput()
    {
        // Right Mouse Button: permanent rotation
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * pitchSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Middle Mouse Button Hold: temporary freelook
        if (Input.GetMouseButtonDown(2))
        {
            tempYaw = yaw;
            tempPitch = pitch;
        }

        if (Input.GetMouseButton(2))
        {
            isMiddleRotating = true;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            tempYaw += mouseX * rotationSpeed;
            tempPitch -= mouseY * pitchSpeed;
            tempPitch = Mathf.Clamp(tempPitch, minPitch, maxPitch);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            // If MMB was a tap (no drag), reset to default angles
            if (!isMiddleRotating || (Mathf.Approximately(Input.GetAxis("Mouse X"), 0f) && Mathf.Approximately(Input.GetAxis("Mouse Y"), 0f)))
            {
                Vector3 resetAngles = Quaternion.LookRotation(-defaultOffset).eulerAngles;
                yaw = resetAngles.y;
                pitch = resetAngles.x;
            }

            isMiddleRotating = false;
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
