using System.Collections;
using UnityEngine;

/// <summary>
/// Always allows the Life Form to walk on water.
/// </summary>
public class Power_WaterWalking : Power
{
    [Tooltip("Water Walking Global Bool")] [SerializeField]
    private GlobalBool waterWalking;

    internal override void OnEnable()
    {
        base.OnEnable();
        AttemptToExecute();
    }

    /// <inheritdoc />
    public override IEnumerator ResetPower()
    {
        StopAllCoroutines();
        waterWalking.CurrentValue = false;
        yield return null;
    }

    /// <inheritdoc />
    public override void AttemptToExecute()
    {
        if (!IsActive())
        {
            StartCoroutine(ExecutePower());
        }
    }

    /// <summary>
    /// Turns on Water Walking.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator ExecutePower()
    {
        waterWalking.CurrentValue = true;
        yield return null;
    }

    /// <summary>
    /// Turns off Water Walking.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator StopPower()
    {
        waterWalking.CurrentValue = false;
        yield return null;
    }

    /// <inheritdoc />
    public override bool IsActive()
    {
        return waterWalking.CurrentValue;
    }
}