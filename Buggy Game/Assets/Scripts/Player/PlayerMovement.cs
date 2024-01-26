using UnityEngine;

/// <summary>
/// - Walk
///     - Modular Speed
///         - Increases based on # of new bugs caught
///         - Analog
///     - Walk on Water
///         - Enables water colliders
/// - Crouch
///     - Becomes smaller
///         - Shrink Hitbox
///         - Lower camera
///         - Needs smooth transition
///     - Moves slower
///     - Needs to be able to detect if there is something above before transitioning to walk
/// - Look Around (First Person)
///     - Standard FP Camera Control
/// - Butterfly Jump
///     - When Butterfly Jumping your movement is locked
///     - Lerps you from point a to point b
/// - Audio Hooks
///     - Different terrain types have different sound effects & different volume
///         - Crouching decreases the sound you make
/// </summary>
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// - Walk
    ///     - Modular Speed
    ///         - Increases based on # of new bugs caught
    ///         - Analog
    ///     - Walk on Water
    ///         - Enables water colliders
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
    /// Private Movement Variables.
    /// </summary>
    private Vector3 _velocity;

    /// <summary>
    /// - Crouch
    ///     - Becomes smaller
    ///         - Shrink Hitbox
    ///         - Lower camera
    ///         - Needs smooth transition
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

    /// <summary>
    /// Inputs.
    /// </summary>
    private float _xInput;

    private float _zInput;

    [Header("Ground Variables")] [Tooltip("Transform that checks for the ground.")] [SerializeField]
    private Transform groundCheck;

    [Tooltip("Distance that counts as touching the ground.")] [SerializeField]
    private GlobalFloat groundDistance;

    [Tooltip("What can we walk on?")] [SerializeField]
    private LayerMask groundMask;

    [Tooltip("Are we on the ground?")] [SerializeField]
    private GlobalBool isGrounded;


    [Header("Required Components")] private CharacterController _characterController;
    private CameraController _cameraController;

    private void Awake()
    {
        waterWalking.OnChanged.AddListener(WaterWalking);
        _characterController = GetComponent<CharacterController>();
        _characterController.height = standingHeight.CurrentValue;
        _cameraController = GetComponent<CameraController>();
        _cameraController.SetUpIsCrouching(isCrouching);
    }

    private void OnDestroy()
    {
        waterWalking.OnChanged.RemoveListener(WaterWalking);
    }

    /// <summary>
    /// Returns the max speed of the player based on the number of bugs they've caught.
    /// </summary>
    /// <returns>The max speed.</returns>
    private float GetMaxSpeed() => speedGraph.Evaluate(BugInventory.NumberOfBugsCaught());

    /// <summary>
    /// Returns the max crouching speed of the player based on the number of bugs they've caught.
    /// </summary>
    /// <returns>The max crouching speed.</returns>
    private float GetMaxCrouchingSpeed() => speedGraphCrouch.Evaluate(BugInventory.NumberOfBugsCaught());

    /// <summary>
    /// Sets our ability to walk on water.
    /// </summary>
    /// <param name="canWalk">Can we walk on water?</param>
    private void WaterWalking(bool canWalk) => groundMask =
        canWalk ? LayerMask.GetMask("Default") & LayerMask.GetMask("Water") : LayerMask.GetMask("Default");

    private void Update()
    {
        MovementController();
    }

    private void MovementController()
    {
        CrouchCheck();
        InfluenceVelocity();
    }

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
            if (leftMovementInputs.PressingOneOfTheKeys())
            {
                _xInput += -1f;
            }

            if (rightMovementInputs.PressingOneOfTheKeys())
            {
                _xInput += 1f;
            }

            if (forwardMovementInputs.PressingOneOfTheKeys())
            {
                _zInput += 1f;
            }

            if (backwardMovementInputs.PressingOneOfTheKeys())
            {
                _zInput += -1f;
            }
        }

        // Movement Vector Controls
        Vector3 moveVector = transform.right * _xInput + transform.forward * _zInput;

        _velocity = isCrouching.CurrentValue
            ? moveVector.normalized * GetMaxCrouchingSpeed()
            : moveVector.normalized * GetMaxSpeed();

        // Gravity
        if (isGrounded.CurrentValue)
        {
            _velocity.y = 0;
        }
        else
        {
            _velocity.y += gravity.CurrentValue * Time.deltaTime;
        }


        _characterController.Move(_velocity * Time.deltaTime);
    }
}