using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SoundGenerator))]
public class LifeFormMovement : MonoBehaviour
{
    [Tooltip("Distance from current position to desired position where we can say we reached")] [SerializeField]
    private GlobalFloat reachedDistance;

    [Tooltip("Gravity Value")] [SerializeField]
    private GlobalFloat gravity;

    [Tooltip("Can this Life Form Fly?")] [SerializeField]
    private GlobalBool canFly;

    [Tooltip("Movement Sound")] [SerializeField]
    private InUniverseSound movementSound;

    [Tooltip("Slow Movement Speed")] [SerializeField]
    private GlobalFloat slowSpeed;

    [Tooltip("Medium Movement Speed")] [SerializeField]
    private GlobalFloat mediumSpeed;

    [Tooltip("Fast Movement Speed")] [SerializeField]
    private GlobalFloat fastSpeed;

    [Header("Debug Mode")] [Tooltip("Make Life Form Move to the Debug Mode")] [SerializeField]
    private bool useDebugPath;

    [Tooltip("Location of the Debug Path")] [SerializeField]
    private Vector3 debugPathLocation = new(0, 0, 0);

    [Tooltip("Turn on Debug Gizmos")] [SerializeField]
    private bool useDebugGizmos;

    private CharacterController _characterController;
    private SoundGenerator _soundGenerator;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _soundGenerator = GetComponent<SoundGenerator>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        if (useDebugPath)
        {
            Move(new List<Vector3> { debugPathLocation });
        }
    }

    /// <summary>
    /// Applies gravity to the Life Form.
    /// </summary>
    private void ApplyGravity()
    {
        Vector3 movementVector = Vector3.zero;
        // Add Gravity if we can't fly
        movementVector.y = canFly && !canFly.CurrentValue
            ? movementVector.y - gravity.CurrentValue * Time.fixedDeltaTime
            : movementVector.y;

        _characterController.Move(movementVector);
    }

    /// <summary>
    /// Uses the fast speed for movement.
    /// </summary>
    /// <param name="path">The path to follow</param>
    public void MoveFast(List<Vector3> path) => Move(path, fastSpeed.CurrentValue);

    /// <summary>
    /// Uses the medium speed for movement.
    /// </summary>
    /// <param name="path">The path to follow</param>
    public void Move(List<Vector3> path) => Move(path, mediumSpeed.CurrentValue);

    /// <summary>
    /// Uses the slow speed for movement.
    /// </summary>
    /// <param name="path">The path to follow</param>
    public void MoveSlow(List<Vector3> path) => Move(path, slowSpeed.CurrentValue);

    /// <summary>
    /// Try to follow the given path.
    /// </summary>
    /// <param name="path">The path to follow.</param>
    /// <param name="speed">The speed to go at.</param>
    private void Move(List<Vector3> path, float speed)
    {
        // Set the movement position
        Vector3 movementPosition = transform.position;
        foreach (Vector3 position in path.Where(position =>
                     Vector3.Distance(transform.position, position) > reachedDistance.CurrentValue))
        {
            movementPosition = position;
            break;
        }

        // If we are close enough to the final position, we don't move
        float distanceToEnd = Vector3.Distance(transform.position, movementPosition);
        if (distanceToEnd <= reachedDistance.CurrentValue)
        {
            Debug.Log($"{name} reached final position.", this);
            return;
        }

        // Rotate to face movement position
        transform.LookAt(movementPosition);

        // Get normalized vector to movement position
        Vector3 normalizedVector = (movementPosition - transform.position).normalized;

        // Create a movement Vector
        Vector3 movementVector = normalizedVector * (speed * Time.fixedDeltaTime);

        if (useDebugGizmos)
        {
            Debug.DrawLine(transform.position, transform.position + movementVector, Color.green, Time.deltaTime);
        }

        // Add force to move
        _characterController.Move(movementVector);
    }

    /// <summary>
    /// Plays the movement sound.
    /// </summary>
    [ContextMenu("Play Movement Sound")]
    public void PlayMovementSound()
    {
        _soundGenerator.PlaySound(movementSound);
    }
}