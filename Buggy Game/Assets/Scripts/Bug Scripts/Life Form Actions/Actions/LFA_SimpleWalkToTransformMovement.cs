using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_SimpleWalkToTransformMovement : LifeFormAction
{
    [Tooltip("The Transform to go to.")] [SerializeField]
    private Transform positionToGoTo;
    
    private LifeFormMovement _lifeFormMovement;
    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
    }

    /// <summary>
    /// Walks to the Transform Position in a straight path.
    /// </summary>
    /// <param name="position">Not used.</param>
    /// <returns>Nothing.</returns>
    public override IEnumerator PerformAction(Vector3 position = new())
    {
        _lifeFormMovement.MoveSlow(new List<Vector3> {positionToGoTo.position});
        yield return null;
    }
}
