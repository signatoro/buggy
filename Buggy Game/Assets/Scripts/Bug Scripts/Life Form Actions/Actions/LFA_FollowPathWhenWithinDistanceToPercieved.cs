using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_FollowPathWhenWithinDistanceToPercieved : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;

    [Tooltip("What to do when the path is completed.")] [SerializeField]
    private UnityEvent onPathComplete = new();

    [Tooltip("Distance Away that this starts following.")] [SerializeField]
    private GlobalFloat distanceStartFollowing;

    [Tooltip("The locations for the path.")] [SerializeField]
    private List<Transform> pathNodes = new();

    private LifeFormMovement _lifeFormMovement;

    private List<Transform> _remainingTransforms = new();

    internal override void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
        _remainingTransforms = new List<Transform>(pathNodes);
        base.Awake();
    }

    /// <summary>
    /// Moves to the place it on the path when it perceives something.
    /// </summary>
    /// <param name="position">The position of the perceived life form.</param>
    /// <returns>Nothing.</returns>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        List<Vector3> nodes = _remainingTransforms.Select(x => x.position).ToList();
        if (Vector3.Distance(position, transform.position) < distanceStartFollowing.CurrentValue &&
            _remainingTransforms.Count > 0)
        {
            bool reached = _lifeFormMovement.Move(nodes[0], speed.CurrentValue);

            if (reached)
            {
                _remainingTransforms.RemoveAt(0);
            }

            if (_remainingTransforms.Count == 0)
            {
                onPathComplete.Invoke();
            }
        }
        else if (_remainingTransforms.Count > 0)
        {
            _lifeFormMovement.Move(nodes[0], 0);
        }

        yield return null;
    }
}