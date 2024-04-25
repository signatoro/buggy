using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_FollowWhenWithinDistance : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;
    
    [Tooltip("What to do when the path is completed.")] [SerializeField]
    private UnityEvent onPathComplete = new();

    [Tooltip("Distance Away that this starts following.")] [SerializeField]
    private GlobalFloat distanceStartFollowing;

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
        if (Vector3.Distance(position, transform.position) < distanceStartFollowing.CurrentValue)
        {
            bool reached = _lifeFormMovement.Move(position, speed.CurrentValue);
            if (reached)
            {
                onPathComplete.Invoke();
            }
        }
        yield return null;
    }
}
