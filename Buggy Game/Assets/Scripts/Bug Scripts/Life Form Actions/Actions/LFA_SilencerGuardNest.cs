using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeFormMovement))]
[RequireComponent(typeof(Sense_Sight))]
public class LFA_SilencerGuardNest : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;

    [Tooltip("Root of the area.")] [SerializeField]
    private Transform areaRoot;

    [Tooltip("Area Radius.")] [SerializeField]
    private GlobalFloat areaRadius;

    [Tooltip("Maximum Allowed Light Value.")] [SerializeField]
    private GlobalFloat maxAllowedLight;

    private Vector3 _currentPositionToReach = new Vector3();

    private LifeFormMovement _lifeFormMovement;

    private Sense_Sight _senseSight;

    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
        _senseSight = GetComponent<Sense_Sight>();
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