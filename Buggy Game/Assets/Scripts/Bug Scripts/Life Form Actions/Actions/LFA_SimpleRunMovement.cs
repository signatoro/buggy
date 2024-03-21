using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_SimpleRunMovement : LifeFormAction
{
    private LifeFormMovement _lifeFormMovement;
    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
    }

    /// <summary>
    /// Runs to the Position in a straight path.
    /// </summary>
    /// <param name="position">The position to run to.</param>
    /// <returns>Nothing.</returns>
    public override IEnumerator PerformAction(Vector3 position = new())
    {
        _lifeFormMovement.MoveFast(new List<Vector3> {position});
        yield return null;
    }
}
