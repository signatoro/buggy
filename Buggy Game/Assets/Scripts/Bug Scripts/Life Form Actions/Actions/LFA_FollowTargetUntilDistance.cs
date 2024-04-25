using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_FollowTargetUntilDistance : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;
    
    [Tooltip("What to do when the path is completed.")] [SerializeField]
    private UnityEvent onPathComplete = new();

    [Tooltip("Distance Away that this stops following.")] [SerializeField]
    private GlobalFloat distanceStopFollowing;

    private LifeFormMovement _lifeFormMovement;

    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
    }
    
    /// <summary>
    /// Moves to the place it perceived.
    /// </summary>
    /// <param name="position">The position to get to.</param>
    /// <returns>Nothing.</returns>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        if (Vector3.Distance(position, transform.position) < distanceStopFollowing.CurrentValue)
        {
            onPathComplete.Invoke();
        }
        else
        {
            _lifeFormMovement.Move(position, speed.CurrentValue);
        }
        yield return null;
    }
}
