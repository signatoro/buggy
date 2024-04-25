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

    [Tooltip("Can we move?")] public bool CanMove { get; set; } = true;

    [Header("Debug Mode")] [Tooltip("Make Life Form Move to the Debug Mode")] [SerializeField]
    private bool useDebugPath;

    [Tooltip("Location of the Debug Path")] [SerializeField]
    private Vector3 debugPathLocation = new(0, 0, 0);

    [Tooltip("Turn on Debug Gizmos")] [SerializeField]
    private bool useDebugGizmos;

    private SoundGenerator _soundGenerator;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _soundGenerator = GetComponent<SoundGenerator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (useDebugPath)
        {
            Move(debugPathLocation, 5);
        }
    }

    /// <summary>
    /// Try to follow the given path.
    /// </summary>
    /// <param name="destination">The destination.</param>
    /// <param name="speed">The speed to go at.</param>
    /// <returns>True if the final position was reached, else false.</returns>
    public bool Move(Vector3 destination, float speed)
    {
        if (!CanMove)
        {
            _navMeshAgent.speed = 0f;
            return false;
        }

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