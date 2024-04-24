using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SoundGenerator))]
[RequireComponent(typeof(NavMeshAgent))]
public class LifeFormMovement : MonoBehaviour
{
    [Tooltip("Distance from current position to desired position where we can say we reached")] [SerializeField]
    private GlobalFloat reachedDistance;

    [Tooltip("Movement Sound")] [SerializeField]
    private InUniverseSound movementSound;

    [Header("Debug Mode")] [Tooltip("Make Life Form Move to the Debug Mode")] [SerializeField]
    private bool useDebugPath;

    [Tooltip("Location of the Debug Path")] [SerializeField]
    private Vector3 debugPathLocation = new(0, 0, 0);

    [Tooltip("Turn on Debug Gizmos")] [SerializeField]
    private bool useDebugGizmos;

    private CharacterController _characterController;
    private SoundGenerator _soundGenerator;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _soundGenerator = GetComponent<SoundGenerator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
       // ApplyGravity();
        if (useDebugPath)
        {
            Move(debugPathLocation, 5);
        }
    }

    /// <summary>
    /// Applies gravity to the Life Form.
    /// </summary>
   /* private void ApplyGravity()
    {
        Vector3 movementVector = Vector3.zero;
        // Add Gravity if we can't fly
        movementVector.y = canFly && !canFly.CurrentValue
            ? movementVector.y - gravity.CurrentValue * Time.fixedDeltaTime
            : movementVector.y;

        _characterController.Move(movementVector);
    }*/

    /// <summary>
    /// Try to follow the given path.
    /// </summary>
    /// <param name="destination">The destination.</param>
    /// <param name="speed">The speed to go at.</param>
    /// <returns>True if the final position was reached, else false.</returns>
    public bool Move(Vector3 destination, float speed)
    {
        // If we are close enough to the final position, we don't move
        float distanceToEnd = Vector3.Distance(transform.position, destination);
        if (distanceToEnd <= reachedDistance.CurrentValue)
        {
            Debug.Log($"{name} reached final position.", this);
            return true;
        }

        _navMeshAgent.speed = speed;
        _navMeshAgent.SetDestination(destination);
        return false;
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