using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LifeFormMovement))]
public class LFA_RunAwayFromTarget : LifeFormAction
{
    [Tooltip("The speed to move at.")] [SerializeField]
    private GlobalFloat speed;
    
    [Tooltip("The amount of time to run away for.")] [SerializeField]
    private GlobalFloat runAwayTime;
    
    [Tooltip("What to do when the time is completed.")] [SerializeField]
    private UnityEvent onTimeComplete = new();

    private LifeFormMovement _lifeFormMovement;

    private void Awake()
    {
        _lifeFormMovement = GetComponent<LifeFormMovement>();
    }

    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        float timer = 0;

        while (timer < runAwayTime.CurrentValue)
        {
            position.y = transform.position.y;
            Vector3 targetToLifeFormVector = (transform.position - position).normalized;
            
            Debug.DrawRay(transform.position, targetToLifeFormVector * 10, Color.red, Time.deltaTime);

            _lifeFormMovement.Move(targetToLifeFormVector * 10, speed.CurrentValue);

            timer += Time.deltaTime;
            yield return null;
        }
        
        onTimeComplete.Invoke();
        yield return null;
    }
}
