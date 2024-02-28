using System.Collections;
using UnityEngine;

public class FlyingHandler : MonoBehaviour
{
    [Tooltip("Graph for position during flight (x(0) = point 1, x(1) = point2)")] [SerializeField]
    private AnimationCurve positioning;

    [Tooltip("Interaction Radius for each Point")] [SerializeField]
    private GlobalFloat radius;

    [Tooltip("Flight Time between Points")] [SerializeField]
    private GlobalFloat flightTime;

    [Tooltip("Point 1")] [SerializeField] private Transform point1;

    [Tooltip("Point 2")] [SerializeField] private Transform point2;

    [Tooltip("Fly Keys")] [SerializeField] private GlobalKeyCodeList flyKeys;

    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        bool playerOnPoint1 = Physics.CheckSphere(point1.position, radius.CurrentValue, LayerMask.GetMask("Player"));

        if (playerOnPoint1 && flyKeys.PressingOneOfTheKeys() && _player.GetComponent<Power_Fly>())
        {
            StartCoroutine(AttemptToFly(_player.GetComponent<Power_Fly>()));
        }
    }

    /// <summary>
    /// Moves the Player from a position to another one.
    /// </summary>
    /// <param name="fly">The Flying Power.</param>
    /// <returns>Nothing.</returns>
    private IEnumerator AttemptToFly(Power_Fly fly)
    {
        if (!fly.IsActive())
        {
            fly.AttemptToExecute();

            float timer = 0;
            while (timer < flightTime.CurrentValue)
            {
                Vector3 adjustedPoint2 = new Vector3(point2.position.x,
                    point1.position.y + positioning.Evaluate(timer / flightTime.CurrentValue),
                    point2.position.z);
                _player.transform.position =
                    Vector3.Lerp(point1.position, adjustedPoint2, timer / flightTime.CurrentValue);
                timer += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(fly.StopPower());
        }

        yield return null;
    }
}