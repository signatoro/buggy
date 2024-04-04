using System.Collections;
using UnityEngine;

/// <summary>
/// Player Only.
/// </summary>
public class Power_Fly : Power
{
    [Tooltip("Player Movement Enabled Global Bool")] [SerializeField]
    private GlobalBool playerMovement;

    private bool _isActive;

    /// <inheritdoc />
    public override IEnumerator ResetPower()
    {
        StopAllCoroutines();
        _isActive = false;
        yield return null;
    }

    /// <inheritdoc />
    public override void AttemptToExecute()
    {
        if (!IsActive()) StartCoroutine(ExecutePower());
    }

    /// <summary>
    /// Disables the movement component while Flying.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator ExecutePower()
    {
        _isActive = true;
        playerMovement.CurrentValue = false;
        yield return null;
    }

    /// <inheritdoc />
    public override IEnumerator StopPower()
    {
        playerMovement.CurrentValue = true;
        _isActive = false;
        yield return null;
    }

    /// <inheritdoc />
    public override bool IsActive() => _isActive;
}