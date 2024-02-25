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

    [Tooltip("Standing Camera Transform")] [SerializeField]
    private Transform standingCameraTransform;

    [Tooltip("Crouching Camera Transform")] [SerializeField]
    private Transform crouchingCameraTransform;

    [Tooltip("Time To Lerp Between Crouching and Standing")] [SerializeField]
    private GlobalFloat crouchLerpTime;

    private GlobalBool _isCrouching;
    private Coroutine _crouchCoroutine;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camera.transform.position = standingCameraTransform.position;
    }

    /// <summary>
    /// Gets the Camera.
    /// </summary>
    public Camera Camera => camera;

    /// <summary>
    /// Sets up the variable for is crouching.
    /// </summary>
    /// <param name="c">The global bool for crouching.</param>
    public void SetUpIsCrouching(GlobalBool c)
    {
        _isCrouching = c;
        _isCrouching.OnChanged.AddListener(HandleCrouch);
    }

    /// <summary>
    /// Starts the coroutine for crouching or standing.
    /// </summary>
    /// <param name="isCrouching">Are we crouching or standing?</param>
    private void HandleCrouch(bool isCrouching)
    {
        if (_crouchCoroutine != null)
        {
            StopCoroutine(_crouchCoroutine);
        }

        _crouchCoroutine = StartCoroutine(isCrouching ? Crouch() : StandUp());
    }

    /// <summary>
    /// Moves the camera to the crouching position.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator Crouch()
    {
        float elapsedTime = 0;
        while (elapsedTime < crouchLerpTime.CurrentValue)
        {
            camera.transform.position = Vector3.Lerp(standingCameraTransform.position,
                crouchingCameraTransform.position,
                elapsedTime / crouchLerpTime.CurrentValue);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camera.transform.position = crouchingCameraTransform.position;
        yield return null;
    }

    /// <summary>
    /// Makes the Camera move to the standing position.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator StandUp()
    {
        float elapsedTime = 0;
        while (elapsedTime < crouchLerpTime.CurrentValue)
        {
            camera.transform.position = Vector3.Lerp(crouchingCameraTransform.position,
                standingCameraTransform.position,
                elapsedTime / crouchLerpTime.CurrentValue);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camera.transform.position = standingCameraTransform.position;
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

    private void FixedUpdate()
    {
        if (cameraDisabled.CurrentValue) return;

        // Get Mouse Inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity.CurrentValue;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity.CurrentValue;

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