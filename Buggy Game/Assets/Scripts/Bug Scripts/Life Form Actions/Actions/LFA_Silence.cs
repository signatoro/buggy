using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Power_Silence))]
public class LFA_Silence : LifeFormAction
{
    [Tooltip("Distance Away that this gets used.")] [SerializeField]
    private GlobalFloat distanceToUse;
    
    private Power_Silence _powerSilence;

    private void Awake()
    {
        _powerSilence = GetComponent<Power_Silence>();
    }

    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        if (Vector3.Distance(position, transform.position) < distanceToUse.CurrentValue)
        {
            _powerSilence.AttemptToExecute();
            Debug.Break();
        }

        yield return null;
    }
}
