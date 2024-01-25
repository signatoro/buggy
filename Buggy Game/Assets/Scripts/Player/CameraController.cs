using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [FormerlySerializedAs("cameraObject")] [Tooltip("Player Camera.")] [SerializeField]
    private Camera camera;

    [Tooltip("Mouse Sensitivity.")] [SerializeField]
    private GlobalFloat mouseSensitivity;

    private float _xRotation = 0f;

    [FormerlySerializedAs("tempDisabled")] [Tooltip("Camera Disabled.")] [SerializeField]
    private GlobalBool cameraDisabled;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (cameraDisabled.CurrentValue) return;

        // Get Mouse Inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity.CurrentValue * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity.CurrentValue * Time.deltaTime;

        //Set Rotation on Camera Vertically
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -85f, 85f);
        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0f);

        // Rotate the Player Horizontally
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetXRotation(float x)
    {
        _xRotation = x;
    }

    // Disable this script's functionality for a brief time without making the mouse visible.
    public void TempDisable(float time)
    {
        StartCoroutine(nameof(TempDisableCo), time);
    }

    IEnumerator TempDisableCo(float time)
    {
        cameraDisabled.CurrentValue = true;
        yield return new WaitForSeconds(time);
        cameraDisabled.CurrentValue = false;
    }
}