using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LFA_FollowTarget : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;
    
    [Tooltip("What to do when the path is completed.")] [SerializeField]
    private UnityEvent onPathComplete = new();

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
        Debug.Log($"Going for Ant 2!", this);
        bool reached = _lifeFormMovement.Move(position, speed.CurrentValue);
        if (reached)
        {
            Debug.Log($"Reached Ant 2!", this);
            onPathComplete.Invoke();
        }

        yield return null;
    }
}
