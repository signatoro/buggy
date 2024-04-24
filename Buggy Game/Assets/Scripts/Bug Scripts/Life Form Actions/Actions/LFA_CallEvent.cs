using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LFA_CallEvent : LifeFormAction
{
    [Tooltip("Event to call.")] [SerializeField] private UnityEvent calledEvent = new();

    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        calledEvent.Invoke();
        yield return null;
    }
}
