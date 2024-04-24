using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_RandomMovementInArea : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;

    [Tooltip("Root of the area.")] [SerializeField]
    private Transform areaRoot;

    [Tooltip("Area Radius.")] [SerializeField]
    private GlobalFloat areaRadius;

    private Vector3 _currentPositionToReach = new Vector3();

    private LifeFormMovement _lifeFormMovement;

    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
    }

    /// <summary>
    /// Picks a random place within an area to go to.
    /// </summary>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        if (_currentPositionToReach == new Vector3())
        {
            float randomX = Random.Range(-1.0f * areaRadius.CurrentValue, areaRadius.CurrentValue);
            float randomZ = Random.Range(-1.0f * areaRadius.CurrentValue, areaRadius.CurrentValue);
            Vector2 twoDVector = new Vector2(randomX, randomZ).normalized;
            _currentPositionToReach = new Vector3(twoDVector.x * areaRadius.CurrentValue, 0f,
                twoDVector.y * areaRadius.CurrentValue) + areaRoot.position;
        }

        bool reached = _lifeFormMovement.Move(_currentPositionToReach, speed.CurrentValue);
        if (reached)
        {
            _currentPositionToReach = new Vector3();
        }

        yield return null;
    }
}