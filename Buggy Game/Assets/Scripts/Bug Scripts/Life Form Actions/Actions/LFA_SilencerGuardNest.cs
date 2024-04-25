using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LifeFormMovement))]
[RequireComponent(typeof(Sense_Sight))]
public class LFA_SilencerGuardNest : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;

    [Tooltip("Maximum Allowed Light Value.")] [SerializeField]
    private GlobalFloat maxAllowedLight;

    private Vector3 _currentPositionToReach = new Vector3();

    private LifeFormMovement _lifeFormMovement;

    private Sense_Sight _senseSight;

    internal override void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
        _senseSight = GetComponent<Sense_Sight>();
        base.Awake();
    }

    /// <summary>
    /// Picks a random place within an area to go to if the light value is okay.
    /// </summary>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        if (_currentPositionToReach == new Vector3() ||
            _senseSight.GetLightIntensityAtPoint(transform.position) >= maxAllowedLight.CurrentValue)
        {
            if (_senseSight.GetLightIntensityAtPoint(transform.position) >= maxAllowedLight.CurrentValue)
            {
                Debug.Log(
                    $"{name} is finding a new spot to move to because the light value was {_senseSight.GetLightIntensityAtPoint(transform.position)}");
            }

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