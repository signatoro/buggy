using System;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public abstract class SenseSystem : MonoBehaviour
{
    [Tooltip("Root of the Sense")] [SerializeField]
    protected Transform root;

    [Tooltip("The radius of the Sense")] [SerializeField]
    protected GlobalFloat radius;

    [Tooltip("Turn on Debug Gizmos")] [SerializeField]
    protected bool enableGizmos;
    
    /// <summary>
    /// Utils that the Sense System can use.
    /// </summary>
    protected Utils Utils;

    protected CatchableLifeForm _catchableLifeForm;

    internal virtual void Awake()
    {
        Utils = Utils.UtilsInstance;
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
    }

    internal virtual void Update()
    {
        
    }

    internal virtual void OnDrawGizmos()
    {
        // Display the current radius as a sphere
        if (enableGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(root.position, radius.CurrentValue);
        }
    }
}
