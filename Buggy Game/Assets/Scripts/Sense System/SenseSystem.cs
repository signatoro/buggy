using System;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public abstract class SenseSystem : MonoBehaviour
{
    /// <summary>
    /// Data that contains all the important information from a Sense.
    /// </summary>
    public class SenseData
    {
        [Tooltip("The Life Form that was sensed")]
        public CatchableLifeForm CatchableLifeForm;

        [Tooltip("The Position of what was Sensed")]
        public Vector3 SensePosition;

        [Tooltip("The Value of the Sense Data")]
        public float Value;

        public SenseData(CatchableLifeForm catchableLifeForm, Vector3 sensePosition, float senseValue)
        {
            CatchableLifeForm = catchableLifeForm;
            SensePosition = sensePosition;
            Value = senseValue;
        }
    }

    [Tooltip("Root of the Sense")] [SerializeField]
    protected Transform root;

    [Tooltip("The radius of the Sense")] [SerializeField]
    protected GlobalFloat radius;

    [Tooltip("Turn on Debug Gizmos")] [SerializeField]
    protected bool enableGizmos;

    protected CatchableLifeForm _catchableLifeForm;

    internal virtual void Awake()
    {
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