using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     - Walk
///     - Modular Speed
///     - Increases based on # of new bugs caught
///     - Analog
///     - Walk on Water
///     - Enables water colliders
///     - Crouch
///     - Becomes smaller
///     - Shrink Hitbox
///     - Lower camera
///     - Needs smooth transition
///     - Moves slower
///     - Needs to be able to detect if there is something above before transitioning to walk
///     - Look Around (First Person)
///     - Standard FP Camera Control
///     - Butterfly Jump
///     - When Butterfly Jumping your movement is locked
///     - Lerps you from point a to point b
///     - Audio Hooks
///     - Different terrain types have different sound effects & different volume
///     - Crouching decreases the sound you make
/// </summary>
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BugInventory))]
[RequireComponent(typeof(SoundGenerator))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    ///     - Walk
    ///     - Modular Speed
    ///     - Increases based on # of new bugs caught
    ///     - Analog
    ///     - Walk on Water
    ///     - Enables water colliders
    /// </summary>
    [Header("Walk Movement Variables")]
    [Tooltip("The speed of the player based on the amount of unique bugs they have caught.")]
    [SerializeField]
    private AnimationCurve speedGraph;

    [Tooltip("Is Movement Enabled?")] [SerializeField]
    private GlobalBool movementEnabled;

    [Tooltip("Can we walk on water?")] [SerializeField]
    private GlobalBool waterWalking;

    [Tooltip("Gravity.")] [SerializeField] private GlobalFloat gravity;

    /// <summary>
    ///     - Crouch
    ///     - Becomes smaller
    ///     - Shrink Hitbox
    ///     - Lower camera
    ///     - Needs smooth transition
    ///     - Moves slower
    ///     - Needs to be able to detect if there is something above before transitioning to walk
    /// </summary>
    [Header("Crouching")]
    [Tooltip("The speed of the player while crouching based on the amount of unique bugs they have caught.")]
    [SerializeField]
    private AnimationCurve speedGraphCrouch;

    [Tooltip("Transform that checks for the ceiling.")] [SerializeField]
    private Transform ceilingCheck;

    [Tooltip("Distance that counts as touching a ceiling.")] [SerializeField]
    private GlobalFloat ceilingDistance;

    [Tooltip("What counts as a ceiling?")] [SerializeField]
    private LayerMask ceilingMask;

    [Tooltip("Standing Height")] [SerializeField]
    private GlobalFloat standingHeight;

    [Tooltip("Crouching Height")] [SerializeField]
    private GlobalFloat crouchingHeight;

    [Tooltip("Are we on the crouching?")] [SerializeField]
    private GlobalBool isCrouching;


    [Header("Movement Input")] [Tooltip("Forward Movement Key")] [SerializeField]
    private GlobalKeyCodeList forwardMovementInputs;

    [Tooltip("Backwards Movement Key")] [SerializeField]
    private GlobalKeyCodeList backwardMovementInputs;

    [Tooltip("Left Movement Key")] [SerializeField]
    private GlobalKeyCodeList leftMovementInputs;

    [Tooltip("Right Movement Key")] [SerializeField]
    private GlobalKeyCodeList rightMovementInputs;

    [Tooltip("Crouch Movement Key")] [SerializeField]
    private GlobalKeyCodeList crouchMovementInputs;

    [Header("Ground Variables")] [Tooltip("Transform that checks for the ground.")] [SerializeField]
    private Transform groundCheck;

    [Tooltip("Distance that counts as touching the ground.")] [SerializeField]
    private GlobalFloat groundDistance;

    [Tooltip("What can we walk on?")] [SerializeField]
    private LayerMask groundMask;

    [Tooltip("Are we on the ground?")] [SerializeField]
    private GlobalBool isGrounded;

    [Tooltip("Are we on water?")] [SerializeField]
    private GlobalBool onWater;

    [Header("Respawn")] [Tooltip("Initial Respawn Point")] [SerializeField]
    private Transform initialRespawnPoint;

    [Tooltip("Respawn Fade Image")] [SerializeField]
    private Image fadeImage;

    [Tooltip("Respawn Fade In Time")] [SerializeField]
    private GlobalFloat respawnFadeInTime;

    [Header("SFX")] [Tooltip("Sound for Walking")] [SerializeField]
    private InUniverseSound walkingSound;

    [Tooltip("Sound for Crouching")] [SerializeField]
    private InUniverseSound crouchingSound;

    [Tooltip("Sound for Walking on Water")] [SerializeField]
    private InUniverseSound walkingSoundWater;

    [Tooltip("Sound for Crouching on Water")] [SerializeField]
    private InUniverseSound crouchingSoundWater;

    [Tooltip("Walking Sound Frequency")] [SerializeField]
    private GlobalFloat walkingSoundFrequency;

    // Local Variables

    /// <summary>
    ///     Required Components
    /// </summary>
    private BugInventoryData _bugInventoryData;

    private CameraController _cameraController;

    private SoundGenerator _soundGenerator;

    private CharacterController _characterController;

    private Transform _currentRespawnPoint;

    /// <summary>
    ///     Private Movement Variables.
    /// </summary>
    private Vector3 _velocity;

    /// <summary>
    ///     Inputs.
    /// </summary>
    private float _xInput;

    private float _zInput;

    private float _timerSound;


    private void Awake()
    {
        waterWalking.OnChanged.AddListener(WaterWalking);
        _characterController = GetComponent<CharacterController>();
        _soundGenerator = GetComponent<SoundGenerator>();
        _characterController.height = standingHeight.CurrentValue;
        _cameraController = GetComponent<CameraController>();
        _cameraController.SetUpIsCrouching(isCrouching);
        _bugInventoryData = GetComponent<BugInventory>().GetBugInventoryData();
        _currentRespawnPoint = initialRespawnPoint;
        _timerSound = walkingSoundFrequency.CurrentValue;
    }

    private void Update()
    {
        MovementController();
    }

    private void OnDestroy()
    {
        waterWalking.OnChanged.RemoveListener(WaterWalking);
    }

    /// <summary>
    ///     Returns the max speed of the player based on the number of bugs they've caught.
    /// </summary>
    /// <returns>The max speed.</returns>
    private float GetMaxSpeed()
    {
        return speedGraph.Evaluate(_bugInventoryData.NumberOfBugsCaught());
    }

    /// <summary>
    ///     Returns the max crouching speed of the player based on the number of bugs they've caught.
    /// </summary>
    /// <returns>The max crouching speed.</returns>
    private float GetMaxCrouchingSpeed()
    {
        return speedGraphCrouch.Evaluate(_bugInventoryData.NumberOfBugsCaught());
    }

    /// <summary>
    ///     Sets our ability to walk on water.
    /// </summary>
    /// <param name="canWalk">Can we walk on water?</param>
    private void WaterWalking(bool canWalk)
    {
        groundMask =
            canWalk ? LayerMask.GetMask("Default") & LayerMask.GetMask("Water") : LayerMask.GetMask("Default");
    }

    private void MovementController()
    {
        CrouchCheck();
        InfluenceVelocity();
        _timerSound -= Time.deltaTime;
        if (_timerSound <= 0)
        {
            AttemptToPlayMovementSound();
        }
    }

    /// <summary>
    ///     Checks if we are crouching and sets values accordingly.
    /// </summary>
    private void CrouchCheck()
    {
        if (crouchMovementInputs.PressingOneOfTheKeys() && !isCrouching.CurrentValue)
        {
            isCrouching.CurrentValue = true;
            _characterController.height = crouchingHeight.CurrentValue;
            float distanceBetweenCenters = standingHeight.CurrentValue / 2f - crouchingHeight.CurrentValue / 2f;
            float centerY = standingHeight.CurrentValue >= crouchingHeight.CurrentValue
                ? _characterController.center.y - distanceBetweenCenters
                : _characterController.center.y + distanceBetweenCenters;
            _characterController.center =
                new Vector3(_characterController.center.x, centerY, _characterController.center.z);
        }
        else if (!Physics.CheckSphere(ceilingCheck.position, ceilingDistance.CurrentValue, ceilingMask) &&
                 isCrouching.CurrentValue && !crouchMovementInputs.PressingOneOfTheKeys())
        {
            isCrouching.CurrentValue = false;
            _characterController.height = standingHeight.CurrentValue;
            _characterController.center = Vector3.zero;
        }
    }

    /// <summary>
    ///     Sets the velocity of the player.
    /// </summary>
    private void InfluenceVelocity()
    {
        // Ground Check
        isGrounded.CurrentValue = Physics.CheckSphere(groundCheck.position, groundDistance.CurrentValue, groundMask);

        // Initialize Inputs
        _xInput = 0f;
        _zInput = 0f;

        if (movementEnabled.CurrentValue)
        {
            // Movement Inputs Controls
            if (leftMovementInputs.PressingOneOfTheKeys()) _xInput += -1f;

            if (rightMovementInputs.PressingOneOfTheKeys()) _xInput += 1f;

            if (forwardMovementInputs.PressingOneOfTheKeys()) _zInput += 1f;

            if (backwardMovementInputs.PressingOneOfTheKeys()) _zInput += -1f;
        }

        // Movement Vector Controls
        Vector3 moveVector = transform.right * _xInput + transform.forward * _zInput;

        _velocity = isCrouching.CurrentValue
            ? moveVector.normalized * GetMaxCrouchingSpeed()
            : moveVector.normalized * GetMaxSpeed();

        // Gravity
        _velocity.y = -1 * gravity.CurrentValue;

        _velocity *= Time.deltaTime;

        // Respawn if on Water when not allowed
        onWater.CurrentValue = Physics.CheckSphere(groundCheck.position,
            groundDistance.CurrentValue, LayerMask.GetMask("Water"));

        if (!isGrounded.CurrentValue && !waterWalking.CurrentValue && onWater.CurrentValue)
        {
            _velocity = Vector3.zero;
            StartCoroutine(Respawn());
        }
        else
        {
            _characterController.Move(_velocity);
        }
    }

    /// <summary>
    /// Attempts to play the sound associated with your current state.
    /// </summary>
    private void AttemptToPlayMovementSound()
    {
        if (Mathf.Abs(_velocity.x) > 0.0005f || Mathf.Abs(_velocity.z) > 0.0005f)
        {
            if (isCrouching.CurrentValue)
            {
                _soundGenerator.PlaySound(onWater.CurrentValue ? crouchingSoundWater : crouchingSound);
            }
            else
            {
                _soundGenerator.PlaySound(onWater.CurrentValue ? walkingSoundWater : walkingSound);
            }

            _timerSound = walkingSoundFrequency.CurrentValue;
        }
        else
        {
            _timerSound = 0;
        }
    }

    /// <summary>
    ///     Sets a new Respawn Point.
    /// </summary>
    /// <param name="newRespawnPoint">The new Respawn Point.</param>
    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        _currentRespawnPoint = newRespawnPoint;
    }

    public IEnumerator Respawn()
    {
        // Sets fade to black
        fadeImage.color = Color.black;

        // Disable Movement
        movementEnabled.CurrentValue = false;

        // Move to Respawn Position
        transform.position = _currentRespawnPoint.position;
        transform.rotation = _currentRespawnPoint.rotation;

        // Fade In
        float timer = 0;

        while (timer < respawnFadeInTime.CurrentValue)
        {
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(1, 0, timer / respawnFadeInTime.CurrentValue);
            fadeImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        // Enable Movement
        movementEnabled.CurrentValue = true;

        yield return null;
    }
}