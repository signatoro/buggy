using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [Tooltip("Debug Mode")] [SerializeField]
    private bool useDebugMode;

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
        if (useDebugMode)
        {
            Debug.DrawLine(transform.position,
                transform.position + Move(new List<Vector3> { Vector3.zero }, Time.deltaTime * 10f), Color.green,
                Time.deltaTime);
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
    /// Try to follow the given path.
    /// </summary>
    /// <param name="path">The path to follow.</param>
    /// <param name="speed">The speed to go at.</param>
    /// <returns>The vector that we are moving.</returns>
    public Vector3 Move(List<Vector3> path, float speed)
    {
        // Set the movement position
        Vector3 movementPosition = transform.position;
        foreach (Vector3 position in path.Where(position =>
                     Vector3.Distance(transform.position, position) > reachedDistance.CurrentValue))
        {
            movementPosition = position;
            break;
        }

        // Rotate to face movement position
        transform.LookAt(movementPosition);

        // Get normalized vector to movement position
        Vector3 normalizedVector = (movementPosition - transform.position).normalized;

        // Create a movement Vector
        Vector3 movementVector = normalizedVector * speed;

        // Add force to move
        _characterController.Move(movementVector);

        return movementVector;
    }

    private void ApplyMovementSound()
    {
    }
}