using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_RandomMovementInArea : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;

    private Vector3 _currentPositionToReach = new Vector3();

    private LifeFormMovement _lifeFormMovement;

    internal override void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
        base.Awake();
    }

    /// <summary>
    /// Picks a random place within an area to go to.
    /// </summary>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        if (_currentPositionToReach == new Vector3())
        {
            float randomX = Random.Range(-1.0f * CatchableLifeForm.Spawner.GetSpawnRadius(),
                CatchableLifeForm.Spawner.GetSpawnRadius());
            float randomZ = Random.Range(-1.0f * CatchableLifeForm.Spawner.GetSpawnRadius(),
                CatchableLifeForm.Spawner.GetSpawnRadius());
            Vector2 twoDVector = new Vector2(randomX, randomZ).normalized;
            _currentPositionToReach = new Vector3(twoDVector.x * CatchableLifeForm.Spawner.GetSpawnRadius(), 0f,
                                          twoDVector.y * CatchableLifeForm.Spawner.GetSpawnRadius()) +
                                      CatchableLifeForm.Spawner.transform.position;
        }

        bool reached = _lifeFormMovement.Move(_currentPositionToReach, speed.CurrentValue);
        if (reached)
        {
            _currentPositionToReach = new Vector3();
        }

        yield return null;
    }
}