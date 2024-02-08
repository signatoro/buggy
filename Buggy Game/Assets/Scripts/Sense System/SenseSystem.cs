using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
[RequireComponent(typeof(LifeFormAction))]
public abstract class SenseSystem : MonoBehaviour
{
    [Tooltip("Root of the Sense")] [SerializeField]
    protected Transform root;

    [Tooltip("The radius of the Sense")] [SerializeField]
    protected GlobalFloat radius;

    [Tooltip("Turn on Debug Gizmos")] [SerializeField]
    protected bool enableGizmos;
    
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
