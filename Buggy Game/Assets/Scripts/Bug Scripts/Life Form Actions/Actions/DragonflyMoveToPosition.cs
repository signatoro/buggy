using UnityEngine;
using UnityEngine.Events;

public class DragonflyMoveToPosition : MonoBehaviour
{
    [Tooltip("Start Location.")] [SerializeField]
    private Transform startLocation;

    [Tooltip("Final Location.")] [SerializeField]
    private Transform finalLocation;

    [Tooltip("What to do when the path is completed.")] [SerializeField]
    private UnityEvent onPathComplete = new();

    [Tooltip("Total Time.")] [SerializeField]
    private GlobalFloat totalTime;

    private float _timer;

    private void Update()
    {
        transform.position =
            Vector3.Lerp(startLocation.position, finalLocation.position, _timer / totalTime.CurrentValue);
        _timer += Time.deltaTime;
        if (_timer >= totalTime.CurrentValue)
        {
            _timer = 0;
            onPathComplete.Invoke();
        }
    }
}