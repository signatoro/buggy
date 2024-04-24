using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_FollowPath : LifeFormAction
{
    [Tooltip("The locations for the path.")] [SerializeField]
    private List<Transform> pathNodes = new();

    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;

    [Tooltip("What to do when the path is completed.")] [SerializeField]
    private UnityEvent onPathComplete = new();

    private LifeFormMovement _lifeFormMovement;

    private List<Transform> _remainingTransforms = new();

    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
        _remainingTransforms = new List<Transform>(pathNodes);
    }

    /// <summary>
    /// Goes to each stop on the path.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Nothing.</returns>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        List<Vector3> nodes = _remainingTransforms.Select(x => x.position).ToList();

        bool reached = _lifeFormMovement.Move(nodes[0], speed.CurrentValue);

        if (reached)
        {
            _remainingTransforms.RemoveAt(0);
        }

        if (_remainingTransforms.Count == 0)
        {
            onPathComplete.Invoke();
        }
        
        yield return null;
    }
}