using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [FormerlySerializedAs("cameraObject")] [Tooltip("Player Camera.")] [SerializeField]
    private Camera camera;

    [Tooltip("Mouse Sensitivity.")] [SerializeField]
    private GlobalFloat mouseSensitivity;

    private float _xRotation;

    [FormerlySerializedAs("tempDisabled")] [Tooltip("Camera Disabled.")] [SerializeField]
    private GlobalBool cameraDisabled;

    [Tooltip("Standing Camera Height")] [SerializeField]
    private GlobalFloat standingCameraHeight;

    [Tooltip("Crouching Camera Height")] [SerializeField]
    private GlobalFloat crouchingCameraHeight;

    [Tooltip("Time To Lerp Between Crouching and Standing")] [SerializeField]
    private GlobalFloat crouchLerpTime;

    private GlobalBool _isCrouching;
    private Coroutine _crouchCoroutine;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetUpIsCrouching(GlobalBool c)
    {
        _isCrouching = c;
        _isCrouching.OnChanged.AddListener(HandleCrouch);
    }

    private void HandleCrouch(bool isCrouching)
    {
        if (_crouchCoroutine != null)
        {
            StopCoroutine(_crouchCoroutine);
        }

        _crouchCoroutine = StartCoroutine(isCrouching ? Crouch() : StandUp());
    }

    private IEnumerator Crouch()
    {
        camera.transform.position =
            new Vector3(transform.position.x, standingCameraHeight.CurrentValue, transform.position.z);
        float elapsedTime = 0;
        while (elapsedTime < crouchLerpTime.CurrentValue)
        {
            camera.transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, crouchingCameraHeight.CurrentValue, transform.position.z),
                elapsedTime / crouchLerpTime.CurrentValue);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camera.transform.position =
            new Vector3(transform.position.x, crouchingCameraHeight.CurrentValue, transform.position.z);
        yield return null;
    }

    private IEnumerator StandUp()
    {
        camera.transform.position =
            new Vector3(transform.position.x, crouchingCameraHeight.CurrentValue, transform.position.z);
        float elapsedTime = 0;
        while (elapsedTime < crouchLerpTime.CurrentValue)
        {
            camera.transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, standingCameraHeight.CurrentValue, transform.position.z),
                elapsedTime / crouchLerpTime.CurrentValue);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camera.transform.position =
            new Vector3(transform.position.x, standingCameraHeight.CurrentValue, transform.position.z);
        yield return null;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDestroy()
    {
        _isCrouching.OnChanged.RemoveListener(HandleCrouch);
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