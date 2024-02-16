using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public abstract class Power : MonoBehaviour
{
    [Tooltip("UI Icon for the Power")] [SerializeField]
    protected Sprite icon;

    private void Awake()
    {
        StartCoroutine(ResetPower());
    }

    private void OnEnable()
    {
        StartCoroutine(ResetPower());
    }

    private void OnDisable()
    {
        StartCoroutine(ResetPower());
    }

    private void OnDestroy()
    {
        StartCoroutine(ResetPower());
    }

    /// <summary>
    /// Resets the Power's Values.
    /// </summary>
    public abstract IEnumerator ResetPower();

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