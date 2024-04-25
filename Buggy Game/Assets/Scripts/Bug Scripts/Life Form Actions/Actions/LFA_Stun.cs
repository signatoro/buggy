using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LFA_Stun : LifeFormAction
{
    [Tooltip("Stun Time.")] [SerializeField]
    private GlobalFloat stunTime;

    [Tooltip("Stun Visuals.")] [SerializeField]
    private List<GameObject> visuals = new();

    [Tooltip("On Stunned Event.")] [SerializeField]
    private UnityEvent onStunned = new();
    
    [Tooltip("On Not Stunned Event.")] [SerializeField]
    private UnityEvent onNotStunned = new();

    private LifeFormBrain _brain;
    
    private void Awake()
    {
        _brain = GetComponent<LifeFormBrain>();
    }

    /// <summary>
    /// Stuns the Life Form.
    /// </summary>
    public void ForceStun()
    {
        _brain.enabled = false;
        StartCoroutine(PerformAction());
    }
    
    /// <summary>
    /// Stun the Life Form for some time.
    /// </summary>
    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        onStunned.Invoke();
        foreach (GameObject visual in visuals)
        {
            visual.SetActive(true);
        }
        
        yield return new WaitForSeconds(stunTime.CurrentValue);

        _brain.enabled = true;
        onNotStunned.Invoke();
        foreach (GameObject visual in visuals)
        {
            visual.SetActive(false);
        }
    }
}
