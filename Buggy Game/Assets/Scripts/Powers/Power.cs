using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public abstract class Power : MonoBehaviour
{
    [Tooltip("UI Icon for the Power")] [SerializeField]
    protected Sprite icon;

    private void Awake()
    {
        ResetPower();
    }

    private void OnEnable()
    {
        ResetPower();
    }

    private void OnDisable()
    {
        ResetPower();
    }

    private void OnDestroy()
    {
        ResetPower();
    }

    /// <summary>
    /// Resets the Power's Values.
    /// </summary>
    public abstract void ResetPower();

    /// <summary>
    /// Attempts to Execute the Power.
    /// </summary>
    public abstract void AttemptToExecute();

    /// <summary>
    /// Executes the Power.
    /// </summary>
    public abstract IEnumerator ExecutePower();

    /// <summary>
    /// Stops Executing the Power.
    /// </summary>
    public abstract IEnumerator StopPower();

    /// <summary>
    /// Is the Power Currently Active?
    /// </summary>
    /// <returns>True if the Power is active, else false.</returns>
    public abstract bool IsActive();
}